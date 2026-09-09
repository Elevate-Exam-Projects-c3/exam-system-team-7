using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Orchestrators;

// EXAM-103 — the Orchestrator pattern (Constitution pillar 1). A Command
// mutates ONE object; the registration FLOW touches several (user + OTP +
// email), so coordination lives here, not inside a Command handler:
//   1. prepares flow data (normalize email, hash password, generate OTP),
//   2. sends one Command per state change, wrapped in ONE transaction,
//   3. publishes a Notification for the side effect (sending the email).
public class RegisterUserOrchestrator : IRequestHandler<RegisterUserCommand, RequestResponse<Guid>>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpGenerator _otpGenerator;

    public RegisterUserOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IOtpGenerator otpGenerator)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _otpGenerator = otpGenerator;
    }

    public async Task<RequestResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Flow-level data preparation. The email is normalized ONCE here so
        // every step (check, save, OTP row, email) sees the same value. The
        // plain password is used exactly once and forgotten — only the hash
        // travels to the Commands.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var passwordHash = _passwordHasher.Hash(request.Password);

        // ONE transaction around the whole flow: both Commands save inside
        // it, so the user and the OTP are committed together or not at all.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            // Step 1 — create the pending user (Command = one object).
            var userResult = await _mediator.Send(
                new CreateUserCommand(request.FullName.Trim(), normalizedEmail, passwordHash),
                cancellationToken);

            if (!userResult.Success)
            {
                return userResult; // 409 — finally rolls the (empty) transaction back
            }

            // Step 2 — create the OTP row. The PLAIN code exists only in
            // memory: hashed for storage, emailed via the Notification.
            var plainOtp = _otpGenerator.GenerateSixDigitOtp();

            await _mediator.Send(
                new CreateEmailVerificationOtpCommand(
                    userResult.Data,
                    normalizedEmail,
                    _passwordHasher.Hash(plainOtp),
                    DateTime.UtcNow.AddMinutes(10)),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            // Step 3 — side effect AFTER the commit: never announce a code
            // for an account that failed to save. The Notification handler
            // calls IEmailSender; the Orchestrator never knows how emails
            // are sent.
            await _mediator.Publish(
                new UserRegisteredNotification(userResult.Data, normalizedEmail, plainOtp),
                cancellationToken);

            // 201 with the user id — the response NEVER contains the
            // password hash or the OTP (user story requirement).
            return RequestResponse<Guid>.Created(
                userResult.Data,
                "Registration successful. Please check your email for the verification code.");
        }
        catch (DbUpdateException)
        {
            // Race-condition safety net: the unique index rejected a
            // duplicate insert between the check and the save (SQL 2601/2627).
            return EmailAlreadyRegistered();
        }
        finally
        {
            if (!committed)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }

    // One 409 shape, used by both the pre-check path and the race-condition catch.
    private static RequestResponse<Guid> EmailAlreadyRegistered()
    {
        return RequestResponse<Guid>.Fail(
            "Email already registered.",
            409,
            new Dictionary<string, string[]>
            {
                [nameof(RegisterUserCommand.Email)] = new[] { "Email already registered." }
            });
    }
}
