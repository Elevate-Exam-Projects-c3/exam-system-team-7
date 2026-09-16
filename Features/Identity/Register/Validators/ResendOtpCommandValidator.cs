using FluentValidation;
using exam_system.Features.Identity.Register.Orchestrators;

namespace exam_system.Features.Identity.Register.Validators;

// Email format only; account checks live in the Orchestrator.
public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");
    }
}
