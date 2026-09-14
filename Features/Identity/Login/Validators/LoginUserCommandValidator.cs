using FluentValidation;
using exam_system.Features.Identity.Login.Orchestrators;

namespace exam_system.Features.Identity.Login.Validators;

// Shape rules only (team convention). Login deliberately does NOT enforce the
// registration password rules: any non-empty password is a valid SHAPE — a
// wrong VALUE is a business outcome ("Invalid email or password") owned by
// the Orchestrator. Enforcing register rules here would change error codes
// and leak password policy to whoever probes the endpoint.
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
