using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Notifications;

// Raised after the reset-code row is committed; its handler emails the code.
public record PasswordResetOtpRequestedNotification(Guid UserId, string Email, string PlainOtp) : INotification;
