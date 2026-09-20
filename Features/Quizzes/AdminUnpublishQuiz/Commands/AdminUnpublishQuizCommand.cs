using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUnpublishQuiz.Commands
{
    public record AdminUnpublishQuizCommand(Guid QuizId) : IRequest<RequestResponse<Guid>>;
    
    
}
