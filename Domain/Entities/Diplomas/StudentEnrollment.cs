using exam_system.Domain.Common;
using exam_system.Domain.Entities.Identity;

namespace exam_system.Domain.Entities.Diplomas;

public class StudentEnrollment : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid DiplomaId { get; set; }
    public Diploma Diploma { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
