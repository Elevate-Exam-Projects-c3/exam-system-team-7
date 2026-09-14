using exam_system.Features.Questions.AdminCreateQuestion.Orchestrators;
using FluentValidation;

namespace exam_system.Features.Questions.Validators {
    public class CreateQuestionsAndOptionsValidator : AbstractValidator<CreateQuestionsAndOptionsOrchestrator> {

        public CreateQuestionsAndOptionsValidator() {

            RuleFor(x => x.Text)
            .NotEmpty();

            RuleFor(x => x.Options)
                          .Must(options => options != null && options.Count >= 2)
                          .WithMessage("Question must have at least 2 options.");

            RuleFor(x => x.Options)
                .Must(options =>
                    options != null &&
                    options.Count(x => x.IsCorrect) == 1)
                .WithMessage("Question must have exactly one correct option.");
        }
    }
}
