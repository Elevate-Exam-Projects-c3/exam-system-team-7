using exam_system.Domain.Common;
using exam_system.Domain.Entities.Attempts;

namespace exam_system.Domain.Entities.Quizzes;

public class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;

    public ICollection<StudentQuestionAnswer> SelectedInAnswers { get; set; } = new List<StudentQuestionAnswer>();
}
