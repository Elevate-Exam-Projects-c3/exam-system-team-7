using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace exam_system.Features.Identity.Shared;

// Real email delivery with MailKit (EXAM-103). Gmail SMTP: host
// smtp.gmail.com, port 587 with STARTTLS, and an APP password (not the
// account password). Feature code never sees this class — it depends on
// IEmailSender only.
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_options.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        // The log records the delivery metadata only — never the OTP body.
        _logger.LogInformation("Email sent to {To} via SMTP {Host}:{Port}", to, _options.Host, _options.Port);
    }
}
