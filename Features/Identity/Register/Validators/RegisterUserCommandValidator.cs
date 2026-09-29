using FluentValidation;
using exam_system.Features.Identity.Register.Orchestrators;
using exam_system.Features.Identity.Shared;

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

        // The register-grade rules live in ONE shared extension — the
        // password-reset flow (EXAM-6) reuses the exact same rules.
        RuleFor(x => x.Password)
            .RegisterPasswordRules();
    }
}
