namespace exam_system.Dtos.Quizes
{
    public class PublishCheckItem
    {
        public string CheckName { get; set; } = string.Empty;
        public bool IsPassed { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Guid> RelatedItems { get; set; } = new ();
    }
}
