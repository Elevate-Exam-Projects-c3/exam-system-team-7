using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.GetQuiz.Queries {
    public record CheckIfQuizExistQuery(Guid QuizId) : IRequest<RequestResponse<bool>> { 

    }
}
