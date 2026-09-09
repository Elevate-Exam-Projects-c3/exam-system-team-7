using MediatR;

namespace exam_system.Features.Identity.Register.Notifications;

// A Notification (event) for side-effect reactions (Constitution pillar 1).
// Team convention (2026-09-10, restored): notification records and their
// handlers live together in the Notifications/ folder of the slice. It
// carries the PLAIN OTP in memory only — it is never stored; the email is
// its single destination.
public record UserRegisteredNotification(Guid UserId, string Email, string PlainOtp) : INotification;
