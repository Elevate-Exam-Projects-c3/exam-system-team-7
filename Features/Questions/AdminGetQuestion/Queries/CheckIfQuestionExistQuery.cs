using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminGetQuestion.Queries {
    public record CheckIfQuestionExistQuery (Guid QuestionId) : IRequest<RequestResponse<bool>> {
    }
}
