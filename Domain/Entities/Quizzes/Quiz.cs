using exam_system.Common.Enums;
using exam_system.Domain.Common;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Attempts;

namespace exam_system.Domain.Entities.Quizzes;

public class Quiz : BaseEntity
{
    public Guid DiplomaId { get; set; }
    public Diploma Diploma { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int PassScore { get; set; } = 60;
    public int? MaxAttempts { get; set; }
    public QuizStatus Status { get; set; } = QuizStatus.Draft;
    public DateTime? PublishedAt { get; set; }

    // Navigations
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
