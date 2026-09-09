using MediatR;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.Register.Handlers;

// Notification handler = side-effect reaction (naming, 2026-09-10: named
// after the event it handles — UserRegisteredNotification + Handler — so the
// file list tells you which event each handler reacts to). It only knows
// IEmailSender — console (dev) or SMTP (production) — so swapping the sender
// never touches the flow.
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
