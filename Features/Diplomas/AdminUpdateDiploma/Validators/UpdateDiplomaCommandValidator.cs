using exam_system.Dtos.Diploma.UpdateDiploma;
using exam_system.Features.Diplomas.AdminUpdateDiploma.Commands;
using exam_system.Features.Diplomas.CommonValidator;
using FluentValidation;

namespace exam_system.Features.Diplomas.AdminUpdateDiploma.Validators
{
    public class UpdateDiplomaCommandValidator
        : AbstractValidator<UpdateDiplomaCommand>
    {
        public UpdateDiplomaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Diploma Id is required.");

            RuleFor(x => x.RequestDto)
                .SetValidator(
                    new DiplomaRequestValidator<UpdateDiplomaDto>());
        }
    }
}
