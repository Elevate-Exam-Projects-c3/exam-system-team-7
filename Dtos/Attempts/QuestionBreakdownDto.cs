namespace exam_system.Dtos.Attempts
{
    public class QuestionBreakdownDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? SelectedOptionText { get; set; }
        public bool IsCorrect { get; set; }
        public List<OptionResultDto> Options { get; set; } = new List<OptionResultDto>();
    }
}
