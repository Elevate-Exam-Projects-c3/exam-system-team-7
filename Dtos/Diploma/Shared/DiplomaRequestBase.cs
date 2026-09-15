namespace exam_system.Dtos.Diploma.Shared
{
    public abstract class DiplomaRequestBase
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
