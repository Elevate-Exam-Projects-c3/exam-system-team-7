using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using FluentValidation;

namespace exam_system.Features.Quizzes.Validators {

    public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand> {
        public UpdateQuizCommandValidator() {


            RuleFor(x => x.UpdateQuiz.Title)
                .MinimumLength(3)
                .MaximumLength(200)
                .When(x => x.UpdateQuiz.Title != null);

            RuleFor(x => x.UpdateQuiz.DurationMinutes)
                .GreaterThan(0)
                .When(x => x.UpdateQuiz.DurationMinutes.HasValue);

            RuleFor(x => x.UpdateQuiz.PassScore)
                .InclusiveBetween(0, 100)
                .When(x => x.UpdateQuiz.PassScore.HasValue);

            RuleFor(x => x.UpdateQuiz.MaxAttempts)
                .GreaterThan(0)
                .When(x => x.UpdateQuiz.MaxAttempts.HasValue);


            RuleFor(x => x.UpdateQuiz.StartDate)
        .Must(startDate => startDate.Date >= DateTime.UtcNow.Date)
        .WithMessage("StartDate must be today or a future date.");

            RuleFor(x => x.UpdateQuiz.EndDate)
                .GreaterThanOrEqualTo(x => x.UpdateQuiz.StartDate)
                .WithMessage("EndDate must be greater than or equal to StartDate.");
        }
    }

}