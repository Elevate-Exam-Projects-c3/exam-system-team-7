using MediatR;

namespace exam_system.Features.Identity.Register.Notifications;

// Raised after registration; its handler emails the verification code.
public record UserRegisteredNotification(Guid UserId, string Email, string PlainOtp) : INotification;
