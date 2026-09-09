using MediatR;

namespace exam_system.Features.Identity.Register.Commands;

// A Notification (event) for side-effect reactions (Constitution pillar 1).
// File placement per the team convention: the slice has no separate
// Notifications folder — the event record lives beside the Commands, and
// its handler is named after the event (UserRegisteredNotificationHandler)
// and lives in Handlers. It carries the PLAIN OTP in memory only — it is
// never stored; the email is its single destination.
public record UserRegisteredNotification(Guid UserId, string Email, string PlainOtp) : INotification;
