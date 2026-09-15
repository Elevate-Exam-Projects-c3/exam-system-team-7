using exam_system.Features.Diplomas.BrowseDiplomas.Queries;
using FluentValidation;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Validators
{
    public class BrowseDiplomasQueryValidator
        : AbstractValidator<BrowseDiplomasQuery>
    {
        public BrowseDiplomasQueryValidator()
        {

            RuleFor(x => x.RequestDto.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}
