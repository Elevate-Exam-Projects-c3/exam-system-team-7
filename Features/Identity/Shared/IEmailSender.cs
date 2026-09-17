namespace exam_system.Features.Identity.Shared;

// Email delivery abstraction; implementations: DevEmailSender, SmtpEmailSender.
public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
