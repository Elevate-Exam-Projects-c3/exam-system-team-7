using exam_system.Dtos.Diploma.Shared;
using FluentValidation;

namespace exam_system.Features.Diplomas.CommonValidator
{
    public class DiplomaRequestValidator<T> : AbstractValidator<T>
        where T : DiplomaRequestBase
    {
        public DiplomaRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MinimumLength(3)
                .WithMessage("Title must be at least 3 characters.")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}
