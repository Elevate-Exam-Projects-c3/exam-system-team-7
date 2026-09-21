namespace exam_system.Dtos.Attempts
{
    public class AttemptDetailDto
    {
        public Guid AttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? Score { get; set; }

        public bool? Passed { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime StartedAt { get; set; }

        public List<QuestionBreakdownDto> Questions { get; set; } = new();
    }
}
