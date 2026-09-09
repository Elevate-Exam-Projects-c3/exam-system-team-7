namespace exam_system.Features.Identity.Shared;

// Abstraction over email delivery (Dependency Inversion). The Register
// handler — and later forgot-password and other flows — depends on this
// interface, never on MailKit or on the console. That is what lets us swap
// the implementation (DevEmailSender now, SmtpEmailSender later) without
// touching any feature code.
public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
