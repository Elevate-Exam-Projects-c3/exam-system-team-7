using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using FluentValidation;

namespace exam_system.Features.Quizzes.Validators {
    public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand> {

        public CreateQuizCommandValidator() {

            RuleFor(x => x.CreateQuiz.Title)
               .NotEmpty()
               .WithMessage("Title is required.")
               .MinimumLength(3)
               .WithMessage("Title must be at least 3 characters.")
               .MaximumLength(200)
               .WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.CreateQuiz.DurationMinutes)
                .GreaterThan(0)
                .WithMessage("DurationMinutes must be greater than 0.");

            RuleFor(x => x.CreateQuiz.PassScore)
                .InclusiveBetween(0, 100)
                .When(x => x.CreateQuiz.PassScore.HasValue)
                .WithMessage("PassScore must be between 0 and 100.");

            RuleFor(x => x.CreateQuiz.MaxAttempts)
                .GreaterThan(0)
                .When(x => x.CreateQuiz.MaxAttempts.HasValue)
                .WithMessage("MaxAttempts must be greater than 0.");

            RuleFor(x => x.CreateQuiz.StartDate)
                 .Must(startDate => startDate.Date >= DateTime.UtcNow.Date)
                 .WithMessage("StartDate must be today or a future date.");

            RuleFor(x => x.CreateQuiz.EndDate)
                .GreaterThanOrEqualTo(x => x.CreateQuiz.StartDate)
                .WithMessage("EndDate must be greater than or equal to StartDate.");
        }
    }
}
