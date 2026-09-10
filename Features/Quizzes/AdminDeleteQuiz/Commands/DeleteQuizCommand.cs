using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminDeleteQuiz.Commands {
    public record DeleteQuizCommand (Guid QuizId) : IRequest<RequestResponse<bool>> {

    }
}
