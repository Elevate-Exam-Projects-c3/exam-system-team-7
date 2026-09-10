using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Features.Diplomas.AdminCreateDiploma.Commands;
using exam_system.Features.Diplomas.CommonValidator;
using FluentValidation;


namespace exam_system.Features.Diplomas.AdminCreateDiploma.Validators
{
    public class CreateDiplomaCommandValidator
        : AbstractValidator<CreateDiplomaCommand>
    {
        public CreateDiplomaCommandValidator()
        {
            RuleFor(x => x.RequestDto)
                .SetValidator(
                    new DiplomaRequestValidator<CreateDiplomaDto>());
        }
    }
}
