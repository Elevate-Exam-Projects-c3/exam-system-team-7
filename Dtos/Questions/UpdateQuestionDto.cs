using exam_system.Dtos.Options;
namespace exam_system.Dtos.Questions {
    public class UpdateQuestionDto {

        public string Text { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public int OrderIndex { get; set; }
        public List<UpdateOptionDto> Options { get; set; }= new();
    }
}
