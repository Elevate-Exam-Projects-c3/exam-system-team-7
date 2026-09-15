using exam_system.Features.Diplomas.EnrollDiploma.Orchestrators;
using FluentValidation;

namespace exam_system.Features.Diplomas.EnrollDiploma.Validators
{
    public class EnrollDiplomaOrchestratorValidator
        : AbstractValidator<EnrollDiplomaOrchestrator>
    {
        public EnrollDiplomaOrchestratorValidator()
        {
            RuleFor(x => x.DiplomaId)
                .NotEmpty()
                .WithMessage("Diploma Id is required.");

            RuleFor(x => x.StudentId)
                .NotEmpty()
                .WithMessage("Student Id is required.");
        }
    }
}
