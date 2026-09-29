namespace exam_system.Features.Identity.ForgotPassword.Dtos;

// PasswordResetOtps row as the forgot-password flow reads it.
public record PasswordResetOtpDto(
    Guid Id,
    Guid UserId,
    string OtpHash,
    int AttemptCount,
    DateTime ExpiresAt,
    DateTime CreatedAt);
