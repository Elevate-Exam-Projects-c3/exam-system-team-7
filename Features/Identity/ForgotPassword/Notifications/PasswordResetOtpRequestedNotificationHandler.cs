using MediatR;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Notifications;

// Emails the password-reset code; only knows IEmailSender, so swapping the
// sender never touches the flow (templating + retry arrive with EXAM-113).
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
        return _emailSender.SendEmailAsync(
            notification.Email,
            "Reset your password",
            $"Your password reset code is: {notification.PlainOtp}. It expires in 10 minutes.",
            cancellationToken);
    }
}
