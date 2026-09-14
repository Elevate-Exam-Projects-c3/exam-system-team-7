using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Notifications;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Orchestrators;

// Registration flow: create the user and the OTP row in one transaction,
// then send the verification email after the commit.
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
        // Normalize the email once; every step below uses the same value.
        // The plain password is only used here, only the hash moves on.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var passwordHash = _passwordHasher.Hash(request.Password);

        // One transaction: the user and the OTP row are saved together or not at all.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            // Create the pending user.
            var userResult = await _mediator.Send(
                new CreateUserCommand(request.FullName.Trim(), normalizedEmail, passwordHash),
                cancellationToken);

            if (!userResult.Success)
            {
                return userResult; // 409: email already registered
            }

            // Create the OTP row; the plain code only lives in memory
            // until the email goes out.
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

            // Send the email only after a successful commit.
            await _mediator.Publish(
                new UserRegisteredNotification(userResult.Data, normalizedEmail, plainOtp),
                cancellationToken);

            // 201 with the new user id.
            return RequestResponse<Guid>.Created(
                userResult.Data,
                "Registration successful. Please check your email for the verification code.");
        }
        catch (DbUpdateException)
        {
            // The unique index rejected a duplicate insert (race with another request).
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

    // Shared by the pre-check and the race-condition catch.
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
