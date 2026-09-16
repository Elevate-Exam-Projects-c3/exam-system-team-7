using MediatR;
using Microsoft.Extensions.Options;
using exam_system.Common.Enums;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Identity.Shared.Queries;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Orchestrators;

// Login flow: credentials check, lockout gates, then token issuance.
public class LoginUserOrchestrator : IRequestHandler<LoginUserCommand, RequestResponse<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;   // lock the account at five failures
    private const int LockoutMinutes = 15;

    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public LoginUserOrchestrator(
        IMediator mediator,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RequestResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize the email like the register flow does.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _mediator.Send(new GetUserByEmailQuery(normalizedEmail), cancellationToken);

        // Same generic message as a wrong password — no field leaks.
        if (user is null)
        {
            return InvalidCredentials();
        }

        // Still locked: reject before verifying anything.
        if (user.LockoutEnd is not null && user.LockoutEnd > DateTime.UtcNow)
        {
            var minutesLeft = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            return RequestResponse<LoginResponse>.Fail(
                $"Account temporarily locked due to too many failed attempts. Try again in {(minutesLeft > 0 ? minutesLeft : 0)} minute(s).",
                400);
        }

        // Gate 3 — wrong password: count it, then answer.
        var passwordMatches = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            // After the limit, every new wrong password re-locks the account.
            var attempt = await _mediator.Send(new RecordFailedLoginAttemptCommand(user.Id), cancellationToken);

            if (attempt.Data >= MaxFailedAttempts)
            {
                await _mediator.Send(new LockUserAccountCommand(user.Id, LockoutMinutes), cancellationToken);
                return RequestResponse<LoginResponse>.Fail(
                    $"Account locked for {LockoutMinutes} minutes due to too many failed login attempts.",
                    400);
            }

            return InvalidCredentials();
        }

        // Correct password ends the failure streak.
        await _mediator.Send(new ResetFailedLoginAttemptsCommand(user.Id), cancellationToken);

        // Pending accounts get an explanatory message, not the generic one.
        if (user.AccountStatus != AccountStatus.Active || !user.EmailConfirmed)
        {
            return RequestResponse<LoginResponse>.Fail(
                "Your account is not activated yet. Please verify your email before logging in.",
                400);
        }

        // Issue the token pair.
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.ToString());

        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        await _mediator.Send(
            new CreateRefreshTokenCommand(
                user.Id,
                refreshTokenValue,
                DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)),
            cancellationToken);

        return RequestResponse<LoginResponse>.Ok(
            new LoginResponse(accessToken, refreshTokenValue),
            "Login successful.");
    }

    // Same generic message for unknown email and wrong password.
    private static RequestResponse<LoginResponse> InvalidCredentials()
    {
        return RequestResponse<LoginResponse>.Fail("Invalid email or password.", 400);
    }
}
