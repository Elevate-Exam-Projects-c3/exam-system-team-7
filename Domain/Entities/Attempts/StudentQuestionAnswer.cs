using exam_system.Domain.Common;
using exam_system.Domain.Entities.Quizzes;

namespace exam_system.Domain.Entities.Attempts;

public class StudentQuestionAnswer : BaseEntity
{
    public Guid AttemptId { get; set; }
    public QuizAttempt Attempt { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public Guid? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }

    public bool? IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
