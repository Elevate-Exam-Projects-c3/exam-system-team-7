namespace exam_system.Dtos.Attempts {
    public class QuizAttemptDto {

        public Guid QuizId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime Deadline { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double? Score { get; set; }
        public bool? Passed { get; set; }




    }
}
