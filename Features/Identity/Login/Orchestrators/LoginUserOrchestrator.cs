using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Login.Orchestrators;

// EXAM-107 + EXAM-108 — Orchestrator for the login flow. Gates, in order,
// each chosen deliberately:
//   1. unknown email    -> generic "Invalid email or password" — the SAME
//                          answer a wrong password gets, so nobody can probe
//                          which emails are registered (anti-enumeration).
//   2. still locked     -> distinct locked message; no verification, no counting.
//   3. wrong password   -> RecordFailedLoginAttemptCommand (mechanics); once the
//                          counter reaches 5 -> LockUserAccountCommand and the
//                          locked message; the generic message before that.
//   4. correct password -> ResetFailedLoginAttemptsCommand — a correct password
//                          ends the consecutive-failure streak even while the
//                          account is still pending; THEN the account state is
//                          checked (pending -> explanatory error, required by
//                          the story, NOT the generic credentials message).
//   5. active+confirmed -> ISSUE THE TOKENS (EXAM-108):
//                          JWT generated via ITokenService (pure computation,
//                          no DB) + ONE RefreshTokens row (7-day TTL) through
//                          CreateRefreshTokenCommand.
// Session policy (team decision, 2026-09-14 — REVERTED from one-session-per-user):
// each login creates its own INDEPENDENT refresh token, so multiple devices can
// stay logged in (laptop + mobile at the same time). Revocation happens at
// logout (EXAM-111: current device) or password reset (EXAM-6: all devices) —
// exactly what the stories describe. Every path still performs ONE write, so
// no explicit transaction is needed here (implicit per SaveChanges).
// The controller maps the response to HTTP and sets the httpOnly cookie.
public class LoginUserOrchestrator : IRequestHandler<LoginUserCommand, RequestResponse<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;   // story rule: 5 consecutive failures
    private const int LockoutMinutes = 15;     // story rule: locked for 15 minutes

    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IGenericRepository<ApplicationUser> _users;

    public LoginUserOrchestrator(
        IMediator mediator,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IGenericRepository<ApplicationUser> users)
    {
        _mediator = mediator;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _users = users;
    }

    public async Task<RequestResponse<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Same canonical value as registration/verify (convention #7).
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _users
            .Get(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        // Gate 1 — unknown email: the SAME generic message a wrong password gets.
        if (user is null)
        {
            return InvalidCredentials();
        }

        // Gate 2 — still locked: reject before any verification, count nothing.
        if (user.LockoutEnd is not null && user.LockoutEnd > DateTime.UtcNow)
        {
            var minutesLeft = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            return RequestResponse<LoginResponse>.Fail(
                $"Account temporarily locked due to too many failed attempts. Try again in {(minutesLeft > 0 ? minutesLeft : 0)} minute(s).",
                400);
        }

        // Gate 3 — credential verification (Strategy pattern: IPasswordHasher).
        var passwordMatches = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            // EXAM-105-style mechanics: increment via the single-object Command,
            // then decide. The lockout is a sliding window — on any attempt
            // AFTER the limit (6th, 7th, ...) the counter is already >= 5, so
            // the account is re-locked for another 15 minutes automatically.
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

        // The password is correct: the failure streak is over — reset it
        // BEFORE the account-state check (the counter counts bad passwords,
        // and a correct password is proof of ownership).
        await _mediator.Send(new ResetFailedLoginAttemptsCommand(user.Id), cancellationToken);

        // Gate 4 — account state (story: explanatory error, NOT invalid-credentials).
        if (user.AccountStatus != AccountStatus.Active || !user.EmailConfirmed)
        {
            return RequestResponse<LoginResponse>.Fail(
                "Your account is not activated yet. Please verify your email before logging in.",
                400);
        }

        // EXAM-108 — token issuance. The JWT is a pure computation (no DB);
        // the refresh token VALUE travels with the command (never logged).
        // Multi-device policy: no revocation here — each login's token lives
        // independently until logout (EXAM-111) or password reset (EXAM-6).
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.ToString());

        // Random 256-bit refresh token — unpredictable; one per login.
        var refreshTokenValue = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

        // One write through its own command = one implicit transaction
        // (the explicit Begin/Commit is only needed for multiple aggregates).
        await _mediator.Send(
            new CreateRefreshTokenCommand(
                user.Id,
                refreshTokenValue,
                DateTime.UtcNow.AddDays(7)),
            cancellationToken);

        // The response carries the access token (15-min TTL) and the
        // refresh token value — the CONTROLLER turns the latter into the
        // httpOnly cookie (HTTP concern lives with HTTP, not in the flow).
        return RequestResponse<LoginResponse>.Ok(
            new LoginResponse(accessToken, refreshTokenValue),
            "Login successful.");
    }

    // One generic shape for both "unknown email" and "wrong password".
    private static RequestResponse<LoginResponse> InvalidCredentials()
    {
        return RequestResponse<LoginResponse>.Fail("Invalid email or password.", 400);
    }
}
