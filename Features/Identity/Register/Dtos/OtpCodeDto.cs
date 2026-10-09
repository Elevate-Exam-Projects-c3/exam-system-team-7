namespace exam_system.Features.Identity.Register.Dtos;

// OtpCodes row as verify and resend read it.
public record OtpCodeDto(
    Guid Id,
    Guid UserId,
    string OtpHash,
    int AttemptCount,
    DateTime ExpiresAt,
    DateTime CreatedAt);
