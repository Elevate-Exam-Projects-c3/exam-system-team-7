namespace exam_system.Dtos.Diploma.BrowseDiplomas
{
    public class BrowseDiplomaItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CompletedQuizzes { get; set; }

        public int TotalQuizzes { get; set; }
    }
}
