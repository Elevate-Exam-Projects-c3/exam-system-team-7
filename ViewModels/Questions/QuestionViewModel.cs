using exam_system.ViewModels.Options;

namespace exam_system.ViewModels.Questions {
    public class QuestionViewModel {

        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public List<QuizOptionViewModel> Options { get; set; } = new();

    }
}
