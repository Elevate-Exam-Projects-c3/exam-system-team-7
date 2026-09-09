namespace exam_system.Features.Identity.Shared;

// Development-only IEmailSender: it "sends" the email by writing it to the
// console, so we can build and test the whole registration flow before the
// real SMTP settings are wired up (next step: SmtpEmailSender with MailKit).
// WARNING: logging a real OTP is acceptable ONLY in development. In
// production the plain code must reach the recipient's mailbox and nowhere
// else — never a log file.
public class DevEmailSender : IEmailSender
{
    private readonly ILogger<DevEmailSender> _logger;

    public DevEmailSender(ILogger<DevEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "===== DEV EMAIL ===== To: {To} | Subject: {Subject} | Body: {Body}",
            to, subject, body);

        return Task.CompletedTask;
    }
}
