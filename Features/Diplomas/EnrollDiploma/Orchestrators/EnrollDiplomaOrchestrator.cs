using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Diplomas.EnrollDiploma.Orchestrators
{
    public record EnrollDiplomaOrchestrator( Guid DiplomaId, Guid StudentId ) : IRequest<RequestResponse<bool>>;
}
