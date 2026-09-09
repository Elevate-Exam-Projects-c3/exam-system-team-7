using MediatR;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.Register.Notifications;

// Notification handler = side-effect reaction. Named after the event it
// handles (UserRegisteredNotification + Handler) so the folder list tells
// you which event each handler reacts to. It only knows IEmailSender —
// console (dev) or SMTP (production) — so swapping the sender never touches
// the flow.
public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
{
    private readonly IEmailSender _emailSender;

    public UserRegisteredNotificationHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        return _emailSender.SendEmailAsync(
            notification.Email,
            "Verify your email",
            $"Your verification code is: {notification.PlainOtp}. It expires in 10 minutes.",
            cancellationToken);
    }
}
