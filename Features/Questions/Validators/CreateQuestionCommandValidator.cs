using exam_system.Features.Questions.AdminCreateQuestion.Commands;
using FluentValidation;

namespace exam_system.Features.Questions.Validators {
    public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand> {

        public CreateQuestionCommandValidator() {


            RuleFor(x => x.Text)
           .NotEmpty()
           .WithMessage("Question text is required.");


            RuleFor(x => x.Options)
                .NotNull()
                .Must(x => x.Count >= 2)
                .WithMessage(
                    "Question must have at least 2 options.");


            RuleFor(x => x.Options)
                .Must(x => x.Count(o => o.IsCorrect) == 1)
                .WithMessage(
                    "Question must have exactly one correct option.");


            RuleForEach(x => x.Options)
                .ChildRules(option => {
                    option.RuleFor(x => x.OptionText)
                        .NotEmpty()
                        .WithMessage(
                            "Option text is required.");
                });
        }
    }
}
