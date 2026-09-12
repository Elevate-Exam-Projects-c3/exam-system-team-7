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

// EXAM-2 internal subtask — lean by design (team review, 2026-09-12):
//   * NO invalidation write: verify reads "latest unused" only, so a prior
//     code is shadowed naturally the moment a newer row exists. (The AC's
//     literal "(IsUsed=true)" wording is an open question for the instructor.)
//   * NO new OTP/email code: both reuse the register flow's pieces —
//     CreateEmailVerificationOtpCommand (one OtpCodes row) and
//     UserRegisteredNotification (the email). The only flow-specific lines
//     are generate+hash, kept here on purpose: the plain OTP lives in
//     orchestrator memory from generation to publish and never travels
//     inside a Command where it could be logged.
public class ResendOtpOrchestrator : IRequestHandler<ResendOtpCommand, RequestResponse>
{
    // Borrowed from the EXAM-6 pattern (password-reset resends): the stories
    // specify no cooldown for verification resends, so we apply the same
    // 30-second rule for consistency. Team decision 2026-09-12.
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

        // Same NEUTRAL answer for every 200 path (EXAM-6 pattern): whether the
        // email is unknown, already active, or we just sent a code — the
        // caller cannot tell which one happened.
        const string neutralMessage = "If this email needs verification, a new code has been sent.";

        // Only a pending account has anything to verify. Anything else (or an
        // unknown email) gets the same neutral 200 with no email sent.
        var user = await _users
            .Get(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null || user.AccountStatus != AccountStatus.Pending)
        {
            return RequestResponse.Ok(neutralMessage);
        }

        // Cooldown on the latest issued code (used or not): the client's
        // countdown mirrors this, but the SERVER enforces it — a modified
        // client cannot spam the mailbox.
        var latest = await _otps
            .Get(o => o.Email == normalizedEmail)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null && latest.CreatedAt.AddSeconds(CooldownSeconds) > DateTime.UtcNow)
        {
            int waitSeconds = (int)Math.Ceiling((latest.CreatedAt.AddSeconds(CooldownSeconds) - DateTime.UtcNow).TotalSeconds);
            return RequestResponse.Fail($"Please wait {(waitSeconds > 0 ? waitSeconds : 0)} seconds before requesting a new code.", 429);
        }

        // REUSE from the register flow: the same single-object command that
        // creates the OtpCodes row, and the same notification that sends the
        // email. One command = one SaveChanges = its own implicit transaction,
        // so no explicit Begin/Commit is needed here.
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
