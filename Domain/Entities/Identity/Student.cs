using exam_system.Domain.Common;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Attempts;

namespace exam_system.Domain.Entities.Identity;

public class Student : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string? StudentCode { get; set; }
    public string? PhoneNumber { get; set; }

    // Navigation for 1-to-many relationships
    public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
