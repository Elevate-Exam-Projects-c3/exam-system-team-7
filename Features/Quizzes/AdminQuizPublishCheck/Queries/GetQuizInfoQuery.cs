using exam_system.Dtos.Quizes;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries
{
    public record GetQuizInfoQuery(Guid QuizId):IRequest<RequestResponse<QuizInfoDto>>;
}
