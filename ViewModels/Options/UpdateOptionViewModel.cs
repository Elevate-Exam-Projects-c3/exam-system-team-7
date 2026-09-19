namespace exam_system.ViewModels.Options {
    public class UpdateOptionViewModel {
        public Guid Id { get; set; }

        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
