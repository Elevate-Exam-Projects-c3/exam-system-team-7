using exam_system.Dtos.Quizes;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Orchestrators {
    public record StartQuizOrchestrator(Guid StudentId,Guid quizId) : IRequest<RequestResponse<StartQuizDto>> {
    }
}
