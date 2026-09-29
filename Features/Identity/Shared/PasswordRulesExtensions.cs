using System.Text;
using FluentValidation;

namespace exam_system.Features.Identity.Shared;

// ONE source for the register-grade password rules — the reset flow must
// enforce exactly the same rules (EXAM-6 AC), so the rules live here and
// both validators call this extension.
public static class PasswordRulesExtensions
{
    public static IRuleBuilderOptions<T, string> RegisterPasswordRules<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            // bcrypt only hashes the first 72 bytes of a password.
            .Must(p => p is null || Encoding.UTF8.GetByteCount(p) <= 72)
            .WithMessage("Password must not exceed 72 bytes (bcrypt input limit).")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
