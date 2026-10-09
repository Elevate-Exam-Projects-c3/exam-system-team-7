using exam_system.Features.Questions.AdminUpdateQuestion.Orchestrators;
using FluentValidation;

namespace exam_system.Features.Questions.Validators {
    public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionOptionsOrchestrator> {

        public UpdateQuestionValidator() {

            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("QuestionId is required.");

            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Question text is required.")
                .MaximumLength(2000);

            RuleFor(x => x.Options)
                .NotNull()
                .WithMessage("Options are required.");

            RuleFor(x => x.Options)
                .Must(options =>
                    options != null &&
                    options.Count >= 2)
                .WithMessage(
                    "Question must have at least 2 options.");

            RuleFor(x => x.Options)
         .Must(options =>
             options != null &&
             options.Count(o => o.IsCorrect) == 1)
         .WithMessage(
             "Question must have exactly one correct option.");

            RuleForEach(x => x.Options)
                .ChildRules(option => {
                    option.RuleFor(x => x.OptionText)
                        .NotEmpty()
                        .WithMessage("Option text is required.");
                });
        }
    }
}
