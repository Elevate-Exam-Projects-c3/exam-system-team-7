using exam_system.Common.Enums;
using exam_system.Domain.Common;

namespace exam_system.Domain.Entities.Identity;

public class ApplicationUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
    public bool EmailConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    // Navigation for 1-to-1 relationship with Student
    public Student? Student { get; set; }

    // Navigation for security tokens
    public ICollection<EmailVerificationOtp> EmailVerificationOtps { get; set; } = new List<EmailVerificationOtp>();
    public ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
