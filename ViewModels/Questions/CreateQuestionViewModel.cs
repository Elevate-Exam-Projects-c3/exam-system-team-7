namespace exam_system.ViewModels.Questions {
    public class CreateQuestionViewModel {

        public string Text { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public int OrderIndex { get; set; }

        public List<CreateOptionViewModel> Options { get; set; } = new();

    }
}
