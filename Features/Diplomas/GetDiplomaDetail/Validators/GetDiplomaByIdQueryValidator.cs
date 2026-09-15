using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using FluentValidation;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Validators
{

    public class GetDiplomaByIdQueryValidator
        : AbstractValidator<GetDiplomaByIdQuery>
    {
        public GetDiplomaByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Diploma Id is required.");
        }
    }
}
