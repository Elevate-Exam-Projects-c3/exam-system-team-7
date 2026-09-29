using FluentValidation;
using exam_system.Features.Identity.ForgotPassword.Orchestrators;

namespace exam_system.Features.Identity.ForgotPassword.Validators;

// Email format + a 6-digit code; business rules live in the Orchestrator.
public class VerifyResetOtpCommandValidator : AbstractValidator<VerifyResetOtpCommand>
{
    public VerifyResetOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .Matches("^[0-9]{6}$").WithMessage("Code must be exactly 6 digits.");
    }
}
