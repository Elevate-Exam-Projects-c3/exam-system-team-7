namespace exam_system.Features.Identity.ForgotPassword.Dtos;

// The reset-token carrier row, as Step 3 reads it.
public record PasswordResetTokenDto(
    Guid Id,
    Guid UserId,
    DateTime? ResetTokenExpiresAt);
