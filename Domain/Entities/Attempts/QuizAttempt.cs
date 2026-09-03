using exam_system.Common.Enums;
using exam_system.Domain.Common;
using exam_system.Domain.Entities.Identity;
using exam_system.Domain.Entities.Quizzes;

namespace exam_system.Domain.Entities.Attempts;

public class QuizAttempt : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime Deadline { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public double? Score { get; set; }
    public bool? Passed { get; set; }

    // Navigations
    public ICollection<StudentQuestionAnswer> Answers { get; set; } = new List<StudentQuestionAnswer>();
}
