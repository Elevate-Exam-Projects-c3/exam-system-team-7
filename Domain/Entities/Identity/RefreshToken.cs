using exam_system.Domain.Common;

namespace exam_system.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public bool IsRevoked { get; set; } = false;
    public string? ReplacedByToken { get; set; }
}
