using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminDeleteQuestion.Commands {
    public record DeleteQuestionCommand (Guid questionId) : IRequest<RequestResponse<bool>> {
    }
}
