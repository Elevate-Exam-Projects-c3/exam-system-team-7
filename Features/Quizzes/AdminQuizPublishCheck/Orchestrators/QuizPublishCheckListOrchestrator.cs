using exam_system.Dtos.Quizes;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Orchestrators
{
    public record QuizPublishCheckListOrchestrator(Guid quizId) : IRequest<RequestResponse<QuizPublishCheckDto>>;
        
}
