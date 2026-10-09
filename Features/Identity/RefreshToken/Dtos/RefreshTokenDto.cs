namespace exam_system.Features.Identity.RefreshToken.Dtos;

// Refresh row as the flow consumes it.
public record RefreshTokenDto(
    Guid Id,
    Guid UserId,
    string UserRole,
    bool IsUsed,
    bool IsRevoked,
    DateTime ExpiresAt);
