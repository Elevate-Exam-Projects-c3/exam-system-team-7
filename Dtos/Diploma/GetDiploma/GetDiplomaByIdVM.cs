namespace exam_system.Dtos.Diploma.GetDiploma
{
    public class GetDiplomaByIdVM
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
