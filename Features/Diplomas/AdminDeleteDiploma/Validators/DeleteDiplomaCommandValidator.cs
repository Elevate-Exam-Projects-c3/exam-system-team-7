using exam_system.Features.Diplomas.AdminDeleteDiploma.Commands;
using FluentValidation;

namespace exam_system.Features.Diplomas.AdminDeleteDiploma.Validators
{
    public class DeleteDiplomaCommandValidator
        : AbstractValidator<DeleteDiplomaCommand>
    {
        public DeleteDiplomaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Diploma Id is required.");
        }
    }
}
