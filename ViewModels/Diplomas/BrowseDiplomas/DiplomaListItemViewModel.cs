namespace exam_system.ViewModels.Diplomas.BrowseDiplomas
{
    public class DiplomaListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalQuizzes { get; set; }
        public int CompletedQuizzes { get; set; }
    }
}
