namespace exam_system.Features.Identity.Shared;

// Email delivery abstraction; implementations: DevEmailSender, SmtpEmailSender.
public interface IEmailSender
{
    // Plain-text delivery (the existing flows).
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);

    // Plain + HTML parts (templated emails, EXAM-113): clients that render
    // HTML use it; the rest fall back to the plain body.
    Task SendEmailAsync(string to, string subject, string body, string htmlBody, CancellationToken cancellationToken);
}
