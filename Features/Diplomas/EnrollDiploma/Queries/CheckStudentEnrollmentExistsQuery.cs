using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Queries
{
    public record CheckStudentEnrollmentExistsQuery(
        Guid StudentId,
        Guid DiplomaId
    ) : IRequest<bool>;
}
