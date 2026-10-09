using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUnpublishQuiz.Orchestrators
{
    public record AdminUnpublishQuizOrchestrator(Guid QuizId) : IRequest<RequestResponse<Guid>>;
    
    
}
