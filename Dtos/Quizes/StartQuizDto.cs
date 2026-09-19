using exam_system.Dtos.Questions;

namespace exam_system.Dtos.Quizes {
    public class StartQuizDto {

        public Guid AttemptId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime Deadline { get; set; }

        public List<QuestionDto> Questions { get; set; } = new();
    }
}
