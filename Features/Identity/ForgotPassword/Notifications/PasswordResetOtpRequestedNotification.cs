using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Notifications;

// Raised after the reset-code row is committed; its handler emails the code.
// LifetimeMinutes travels along so the email TEXT can never drift from the
// expiry the Orchestrator actually wrote (same TTL bug fixed in the login flow).
public record PasswordResetOtpRequestedNotification(Guid UserId, string Email, string PlainOtp, int LifetimeMinutes) : INotification;
