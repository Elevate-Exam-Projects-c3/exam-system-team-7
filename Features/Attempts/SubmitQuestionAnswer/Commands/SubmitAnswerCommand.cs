using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Commands {
    public record SubmitAnswerCommand (
        Guid AttemptId,
        Guid QuestionId,
        Guid? SelectedOptionId) : IRequest<RequestResponse<bool>>;
}