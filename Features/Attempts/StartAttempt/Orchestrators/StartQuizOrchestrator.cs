using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Orchestrators {
    public record StartQuizOrchestrator(Guid quizId) : IRequest {
    }
}
