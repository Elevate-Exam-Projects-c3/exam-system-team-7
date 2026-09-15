using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminGetQuiz.Queries {
    public record CheckQuizStatusQuery(Guid QuizId) : IRequest<RequestResponse<bool>> {

    }
}
