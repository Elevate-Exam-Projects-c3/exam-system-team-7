using MediatR;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Queries;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Orchestrators;

// OTP verification: check the latest unused code, consume it and activate
// the user in one transaction. Wrong codes count toward the 5-attempt lock.
public class VerifyOtpOrchestrator : IRequestHandler<VerifyOtpCommand, RequestResponse<Guid>>
{
    private const int MaxAttempts = 5;

    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public VerifyOtpOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RequestResponse<Guid>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Same canonical value as registration (Trim + ToLowerInvariant).
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Only the most recently issued unused OTP is valid.
        var otp = await _mediator.Send(new GetLatestOtpByEmailQuery(normalizedEmail, UnusedOnly: true), cancellationToken);

        // Generic answer: never reveal whether the email exists.
        if (otp is null)
        {
            return InvalidCode();
        }

        // Locked after five wrong attempts.
        if (otp.AttemptCount >= MaxAttempts)
        {
            return RequestResponse<Guid>.Fail(
                "Code locked after too many attempts. Please request a new one.",
                400);
        }

        // Expired gets its own message so the user requests a new code.
        if (otp.ExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse<Guid>.Fail(
                "Code expired. Please request a new one.",
                400);
        }

        // Wrong code: count the attempt first, then answer
        // (locked message at 5, generic before that).
        if (!_passwordHasher.Verify(request.Code, otp.OtpHash))
        {
            var attemptResult = await _mediator.Send(
                new RecordWrongOtpAttemptCommand(otp.Id),
                cancellationToken);

            if (attemptResult.Data >= MaxAttempts)
            {
                return RequestResponse<Guid>.Fail(
                    "Code locked after too many attempts. Please request a new one.",
                    400);
            }

            return InvalidCode();
        }

        // Consume the OTP and activate the user in one transaction.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            // Mark the OTP as used.
            await _mediator.Send(new ConsumeEmailVerificationOtpCommand(otp.Id), cancellationToken);

            // Activate the user.
            await _mediator.Send(new ActivateUserCommand(otp.UserId), cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            // 200: state changed, nothing new was created.
            return RequestResponse<Guid>.Ok(
                otp.UserId,
                "Email verified successfully. Your account is now active.");
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
    private static RequestResponse<Guid> InvalidCode()
    {
        return RequestResponse<Guid>.Fail("Invalid code.", 400);
    }
}
