using System.Text;
using FluentValidation;
using exam_system.Features.Identity.Register.Orchestrators;

namespace exam_system.Features.Identity.Register.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            // bcrypt only reads the first 72 BYTES of input and silently drops
            // the rest — and one non-ASCII letter can be more than one byte.
            // We reject longer passwords instead of letting them be cut quietly.
            // (p is null: NotEmpty already reports missing passwords — every rule
            // in the chain runs, so this one must not crash on null.)
            .Must(p => p is null || Encoding.UTF8.GetByteCount(p) <= 72)
            .WithMessage("Password must not exceed 72 bytes (bcrypt input limit).")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
