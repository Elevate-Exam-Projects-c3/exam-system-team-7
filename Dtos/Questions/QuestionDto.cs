using exam_system.Dtos.Options;

namespace exam_system.Dtos.Questions {
    public class QuestionDto {
        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public List<QuizOptionDto> Options { get; set; } = new();

    }
}
