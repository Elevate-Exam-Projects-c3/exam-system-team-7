namespace exam_system.Features.Identity.Shared;

// Development-only: writes the email to the console. Never register this
// implementation in production — real OTPs must not be logged.
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
