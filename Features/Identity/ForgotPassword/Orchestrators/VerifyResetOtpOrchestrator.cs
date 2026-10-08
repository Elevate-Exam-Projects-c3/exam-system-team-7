using MediatR;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Queries;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 2 of the reset flow: check the latest unused code, then consume it
// and stamp the short-lived reset token in one transaction.
public class VerifyResetOtpOrchestrator : IRequestHandler<VerifyResetOtpCommand, RequestResponse<string>>
{
    private const int MaxAttempts = 5;
    private const int ResetTokenLifetimeMinutes = 10;

    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public VerifyResetOtpOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<RequestResponse<string>> Handle(VerifyResetOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Only the most recently issued unused code is valid.
        var otp = await _mediator.Send(
            new GetLatestPasswordResetOtpByEmailQuery(normalizedEmail, UnusedOnly: true),
            cancellationToken);

        if (otp is null)
        {
            return InvalidCode();
        }

        if (otp.AttemptCount >= MaxAttempts)
        {
            return Locked();
        }

        // Expired gets its own message so the user requests a new code.
        if (otp.ExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse<string>.Fail(
                "Code expired. Please request a new one.",
                400);
        }

        if (!_passwordHasher.Verify(request.Code, otp.OtpHash))
        {
            var attemptResult = await _mediator.Send(
                new RecordWrongPasswordResetOtpAttemptCommand(otp.Id),
                cancellationToken);

            if (attemptResult.Data >= MaxAttempts)
            {
                return Locked();
            }

            return InvalidCode();
        }

        // Same 256-bit crypto-random generator the refresh token uses — the
        // reset token needs the identical entropy (hashing it is pointless
        // at that size; that is why the 6-digit OTP gets hashed instead).
        var resetToken = _tokenService.GenerateRefreshToken();

        // One write: the row flips to used AND starts carrying the token
        // for Step 3 (its own columns, stamped with a lifetime).
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            await _mediator.Send(
                new ConsumePasswordResetOtpCommand(
                    otp.Id,
                    resetToken,
                    DateTime.UtcNow.AddMinutes(ResetTokenLifetimeMinutes)),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            // The token rides the RESPONSE body (the user just proved email
            // ownership in this same session) — no email needed here.
            return RequestResponse<string>.Ok(
                resetToken,
                "Code verified. Use the reset token to set a new password.");
        }
        finally
        {
            if (!committed)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }

    // Deliberately vague for both missing and wrong codes.
    private static RequestResponse<string> InvalidCode()
    {
        return RequestResponse<string>.Fail("Invalid code.", 400);
    }

    private static RequestResponse<string> Locked()
    {
        return RequestResponse<string>.Fail(
            "Code locked after too many attempts. Please request a new one.",
            400);
    }
}
