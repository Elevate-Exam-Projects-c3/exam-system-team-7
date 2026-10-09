using exam_system.ViewModels.Options;

namespace exam_system.ViewModels.Questions {
    public class UpdateQuestionViewModel {
        public string Text { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public int OrderIndex { get; set; }

        public List<UpdateOptionViewModel> Options { get; set; }= new();

    }
}
