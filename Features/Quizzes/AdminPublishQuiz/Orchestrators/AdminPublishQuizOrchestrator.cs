using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Orchestrators
{
    public record AdminPublishQuizOrchestrator(Guid quizId) : IRequest<RequestResponse<Guid>>;
}
