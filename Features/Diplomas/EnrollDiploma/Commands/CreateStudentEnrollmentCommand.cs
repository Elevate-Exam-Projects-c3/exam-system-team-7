using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Commands
{
    public record CreateStudentEnrollmentCommand( Guid StudentId, Guid DiplomaId
    ) : IRequest<RequestResponse<bool>>;
}
