using exam_system.ViewModels.Questions;

namespace exam_system.Dtos.Quizes {
    public class StartQuizViewModel {

        public Guid AttemptId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime Deadline { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}
