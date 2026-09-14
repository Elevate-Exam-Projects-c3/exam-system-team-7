using exam_system.Common.Enums;

namespace exam_system.Dtos.Quizes {
    public class QuizDetailsDto {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Instructions { get; set; } = string.Empty;

        public QuizStatus? Status { get; set; }
        public int? DurationMinutes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxAttempts { get; set; }
    }
}
