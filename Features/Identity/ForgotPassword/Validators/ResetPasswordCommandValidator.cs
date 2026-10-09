using FluentValidation;
using exam_system.Features.Identity.ForgotPassword.Orchestrators;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Validators;

// Token presence + THE SAME password rules as registration (one shared
// rule source) + confirmation match.
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .RegisterPasswordRules();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}
