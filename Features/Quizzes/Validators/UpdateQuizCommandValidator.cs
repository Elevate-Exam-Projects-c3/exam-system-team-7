using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using FluentValidation;

namespace exam_system.Features.Quizzes.Validators {

    public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand> {
        public UpdateQuizCommandValidator() {


            RuleFor(x => x.Title)
                .MinimumLength(3)
                .MaximumLength(200)
                .When(x => x.Title != null);

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0)
                .When(x => x.DurationMinutes.HasValue);

            RuleFor(x => x.PassScore)
                .InclusiveBetween(0, 100)
                .When(x => x.PassScore.HasValue);

            RuleFor(x => x.MaxAttempts)
                .GreaterThan(0)
                .When(x => x.MaxAttempts.HasValue);


            RuleFor(x => x.StartDate)
        .Must(startDate => startDate >= DateTime.UtcNow.Date)
        .WithMessage("StartDate must be today or a future date.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must be greater than or equal to StartDate.");
        }
    }

}