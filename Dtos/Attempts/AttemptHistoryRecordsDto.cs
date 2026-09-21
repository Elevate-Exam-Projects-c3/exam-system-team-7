namespace exam_system.Dtos.Attempts
{
    public class AttemptHistoryRecordsDto
    {
        public Guid AttemptId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
