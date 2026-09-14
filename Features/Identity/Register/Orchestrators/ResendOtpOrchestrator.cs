using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Notifications;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Orchestrators;

// Resend the verification OTP. Only pending accounts get a new code; the
// old one is shadowed because verify only reads the latest unused row.
public class ResendOtpOrchestrator : IRequestHandler<ResendOtpCommand, RequestResponse>
{
    private const int CooldownSeconds = 30;

    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpGenerator _otpGenerator;
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IGenericRepository<EmailVerificationOtp> _otps;

    public ResendOtpOrchestrator(
        IMediator mediator,
        IPasswordHasher passwordHasher,
        IOtpGenerator otpGenerator,
        IGenericRepository<ApplicationUser> users,
        IGenericRepository<EmailVerificationOtp> otps)
    {
        _mediator = mediator;
        _passwordHasher = passwordHasher;
        _otpGenerator = otpGenerator;
        _users = users;
        _otps = otps;
    }

    public async Task<RequestResponse> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Same neutral answer for every 200 path — the caller cannot tell
        // whether the email exists or a code was actually sent.
        const string neutralMessage = "If this email needs verification, a new code has been sent.";

        // Only a pending account needs a code; anything else gets the
        // neutral answer with no email sent.
        var user = await _users
            .Get(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || user.AccountStatus != AccountStatus.Pending)
        {
            return RequestResponse.Ok(neutralMessage);
        }

        // Cooldown: the server enforces it, not just the client UI.
        var latest = await _otps
            .Get(o => o.Email == normalizedEmail)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null && latest.CreatedAt.AddSeconds(CooldownSeconds) > DateTime.UtcNow)
        {
            int waitSeconds = (int)Math.Ceiling((latest.CreatedAt.AddSeconds(CooldownSeconds) - DateTime.UtcNow).TotalSeconds);
            return RequestResponse.Fail($"Please wait {(waitSeconds > 0 ? waitSeconds : 0)} seconds before requesting a new code.", 429);
        }

        var plainOtp = _otpGenerator.GenerateSixDigitOtp();

        await _mediator.Send(
            new CreateEmailVerificationOtpCommand(
                user.Id,
                normalizedEmail,
                _passwordHasher.Hash(plainOtp),
                DateTime.UtcNow.AddMinutes(10)),
            cancellationToken);

        await _mediator.Publish(
            new UserRegisteredNotification(user.Id, normalizedEmail, plainOtp),
            cancellationToken);

        return RequestResponse.Ok(neutralMessage);
    }
}
