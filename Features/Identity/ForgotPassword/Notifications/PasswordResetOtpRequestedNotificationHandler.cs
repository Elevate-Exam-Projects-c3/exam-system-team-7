using MediatR;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Notifications;

// Emails the password-reset code via the shared template; only knows
// IEmailSender, so swapping the sender never touches the flow.
public class PasswordResetOtpRequestedNotificationHandler
    : INotificationHandler<PasswordResetOtpRequestedNotification>
{
    private readonly IEmailSender _emailSender;

    public PasswordResetOtpRequestedNotificationHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task Handle(PasswordResetOtpRequestedNotification notification, CancellationToken cancellationToken)
    {
        var template = new PasswordResetOtpEmailTemplate(notification.PlainOtp, notification.LifetimeMinutes);

        return _emailSender.SendEmailAsync(
            notification.Email,
            template.Subject,
            template.PlainBody,
            template.HtmlBody,
            cancellationToken);
    }
}
