using exam_system.Dtos.Quizes;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries
{
    public record GetQuestionsCorrectOptionCountQuery(Guid quizId) : IRequest<RequestResponse<List<QuestionCorrectOptionCountDto>>>;
}
