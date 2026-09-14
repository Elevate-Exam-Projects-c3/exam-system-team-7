using FluentValidation;
using exam_system.Features.Identity.Login.Orchestrators;

namespace exam_system.Features.Identity.Login.Validators;

// Login only requires a valid-shaped email and a non-empty password —
// wrong values are handled by the Orchestrator.
public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
