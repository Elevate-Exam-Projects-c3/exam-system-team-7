using FluentValidation;
using exam_system.Features.Identity.Register.Orchestrators;

namespace exam_system.Features.Identity.Register.Validators;

// Shape rules only (team convention): a valid email format. Everything else
// (does the account exist, is it pending, cooldown) needs the database and
// lives in the Orchestrator.
public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");
    }
}
