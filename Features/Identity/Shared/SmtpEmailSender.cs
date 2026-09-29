using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace exam_system.Features.Identity.Shared;

// SMTP email delivery with MailKit (STARTTLS).
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
        => SendCoreAsync(to, subject, body, htmlBody: null, cancellationToken);

    public Task SendEmailAsync(string to, string subject, string body, string htmlBody, CancellationToken cancellationToken)
        => SendCoreAsync(to, subject, body, htmlBody, cancellationToken);

    private async Task SendCoreAsync(string to, string subject, string body, string? htmlBody, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_options.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        // multipart/alternative (EXAM-113): modern clients render the HTML
        // part; older ones fall back to the plain text.
        var bodyBuilder = new BodyBuilder { TextBody = body };
        if (htmlBody is not null)
        {
            bodyBuilder.HtmlBody = htmlBody;
        }
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        // Log the delivery metadata only — never the message body.
        _logger.LogInformation("Email sent to {To} via SMTP {Host}:{Port}", to, _options.Host, _options.Port);
    }
}
