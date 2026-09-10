namespace exam_system.Dtos.Quizes {
    public class CreateQuizDto {

        public Guid DiplomaId { get; set; }

        public string Title { get; set; } = null!;

        public string? Instructions { get; set; }

        public int DurationMinutes { get; set; }

        public int? PassScore { get; set; }

        public int? MaxAttempts { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
