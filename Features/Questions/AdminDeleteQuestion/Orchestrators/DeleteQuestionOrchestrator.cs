using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminDeleteQuestion.Orchestrators {
    public record DeleteQuestionOrchestrator (Guid quizId, Guid questionId) : IRequest<RequestResponse<bool>>  {
    }
}
