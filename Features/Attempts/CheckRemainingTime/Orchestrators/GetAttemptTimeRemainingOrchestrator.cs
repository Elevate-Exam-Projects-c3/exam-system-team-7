using exam_system.Dtos.Attempts;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.CheckRemainingTime.Orchestrators {
    public record GetAttemptTimeRemainingOrchestrator (Guid attemptId): IRequest<RequestResponse<AttemptTimeRemainingDto>>{
    }
}
