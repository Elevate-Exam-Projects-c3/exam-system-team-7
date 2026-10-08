using MediatR;
using Microsoft.Extensions.Logging;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Notifications;
using exam_system.Features.Identity.ForgotPassword.Queries;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Identity.Shared.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 1 of the reset flow: issue (or resend) the password-reset OTP.
// Every 200 path answers the SAME neutral message, so the caller cannot
// tell whether the email exists (anti-enumeration, story requirement).
public class ForgotPasswordOrchestrator : IRequestHandler<ForgotPasswordCommand, RequestResponse>
{
    private const int CooldownSeconds = 30;
    private const int OtpLifetimeMinutes = 10;

    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpGenerator _otpGenerator;
    private readonly ILogger<ForgotPasswordOrchestrator> _logger;

    public ForgotPasswordOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IOtpGenerator otpGenerator,
        ILogger<ForgotPasswordOrchestrator> logger)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _otpGenerator = otpGenerator;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        const string neutralMessage = "If this email is registered, a code has been sent.";

        // Only an existing account gets a code; anything else gets the same
        // neutral answer with no email sent.
        var user = await _mediator.Send(new GetUserByEmailQuery(normalizedEmail), cancellationToken);

        if (user is null)
        {
            return RequestResponse.Ok(neutralMessage);
        }

        // The 30-second interval is enforced HERE, per account — the client
        // countdown is cosmetic (a curl call never sees any UI).
        var latest = await _mediator.Send(
            new GetLatestPasswordResetOtpByEmailQuery(normalizedEmail),
            cancellationToken);

        if (latest is not null && latest.CreatedAt.AddSeconds(CooldownSeconds) > DateTime.UtcNow)
        {
            int waitSeconds = (int)Math.Ceiling((latest.CreatedAt.AddSeconds(CooldownSeconds) - DateTime.UtcNow).TotalSeconds);
            return RequestResponse.Fail(
                $"Please wait {(waitSeconds > 0 ? waitSeconds : 0)} seconds before requesting a new code.",
                429);
        }

        var plainOtp = _otpGenerator.GenerateSixDigitOtp();

        // Two writes (clear a possibly-live reset token + insert the new
        // code) must land together or not at all.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            // A new code supersedes any reset token still waiting for Step 3:
            // at most ONE live reset token may exist per account.
            await _mediator.Send(new InvalidateStaleResetTokensCommand(normalizedEmail), cancellationToken);

            await _mediator.Send(
                new CreatePasswordResetOtpCommand(
                    user.Id,
                    normalizedEmail,
                    _passwordHasher.Hash(plainOtp),
                    DateTime.UtcNow.AddMinutes(OtpLifetimeMinutes)),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            // The email goes out only after a successful commit — and its
            // ULTIMATE failure (retries exhausted inside the ResilientEmailSender)
            // answers the client with ONE generic message: zero provider details
            // (EXAM-113). The committed row expires on its own, so a retry
            // after the cooldown starts clean.
            try
            {
                await _mediator.Publish(
                    new PasswordResetOtpRequestedNotification(
                        user.Id,
                        normalizedEmail,
                        plainOtp,
                        OtpLifetimeMinutes),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Password-reset OTP email ultimately failed for {Email}",
                    normalizedEmail);

                return RequestResponse.Fail(
                    "We could not send the email right now. Please try again shortly.",
                    500);
            }

            return RequestResponse.Ok(neutralMessage);
        }
        finally
        {
            if (!committed)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }
}
