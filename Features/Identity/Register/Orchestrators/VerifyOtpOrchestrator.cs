using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Orchestrators;

// EXAM-104 + EXAM-105 — Orchestrator pattern: verify-otp touches TWO
// aggregates (the OtpCodes row and the ApplicationUser), so coordination
// lives here:
//   1. READ: the most recently issued unused OTP for the email,
//   2. GATES in order: exists? locked? expired (DISTINCT message)? hash match?
//   3. ONE transaction: consume the OTP + activate the user.
// EXAM-105: a WRONG code sends RecordWrongOtpAttemptCommand (single-object
// increment) BEFORE answering; once the counter reaches 5 the locked message
// replaces the generic one. Expired/locked/missing attempts do NOT count.
public class VerifyOtpOrchestrator : IRequestHandler<VerifyOtpCommand, RequestResponse<Guid>>
{
    private const int MaxAttempts = 5;

    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IGenericRepository<Domain.Entities.Identity.EmailVerificationOtp> _otps;

    public VerifyOtpOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IGenericRepository<Domain.Entities.Identity.EmailVerificationOtp> otps)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _otps = otps;
    }

    public async Task<RequestResponse<Guid>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Same canonical value as registration (Trim + ToLowerInvariant).
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // "Only the most recently issued OTP for a given email is valid":
        // take the newest row that is still unused. The global soft-delete
        // filter is applied automatically.
        var otp = await _otps
            .Get(o => o.Email == normalizedEmail && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        // Gate 1 — no pending OTP for this email: GENERIC failure only.
        // Never reveal whether the email exists (enumeration protection).
        if (otp is null)
        {
            return InvalidCode();
        }

        // Gate 2 — already locked by previous submissions (no further
        // counting: the counter is already at the limit).
        if (otp.AttemptCount >= MaxAttempts)
        {
            return RequestResponse<Guid>.Fail(
                "Code locked after too many attempts. Please request a new one.",
                400);
        }

        // Gate 3 — expired: DISTINCT message, as the story demands
        // (not the generic invalid-code error).
        if (otp.ExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse<Guid>.Fail(
                "Code expired. Please request a new one.",
                400);
        }

        // Gate 4 — hash comparison with the same bcrypt service that hashed
        // it at registration. A wrong code FIRST counts as an attempt
        // (EXAM-105: every wrong submission increments AttemptCount via its
        // own single-object Command), THEN we answer: locked message once the
        // counter reaches 5, generic message before that. Expired/locked/
        // missing attempts never reach this gate, so they never count.
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

        // ONE transaction: both mutations commit together or not at all.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            // Step 1 — consume the OTP row (one object).
            await _mediator.Send(new ConsumeEmailVerificationOtpCommand(otp.Id), cancellationToken);

            // Step 2 — activate the user (one object).
            await _mediator.Send(new ActivateUserCommand(otp.UserId), cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            // 200 — the state changed, nothing new was created. The response
            // NEVER contains the password hash or the OTP.
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

    // One generic shape for "no OTP / wrong code" — deliberately vague.
    private static RequestResponse<Guid> InvalidCode()
    {
        return RequestResponse<Guid>.Fail("Invalid code.", 400);
    }
}
