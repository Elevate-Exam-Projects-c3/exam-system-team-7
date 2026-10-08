using FluentValidation;
using exam_system.Features.Identity.ForgotPassword.Orchestrators;

namespace exam_system.Features.Identity.ForgotPassword.Validators;

// Email format only; account checks live in the Orchestrator.
public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");
    }
}
