using MediatR;

namespace exam_system.Features.Quizzes.AdminDeleteQuiz.Commands {
    public record DeleteQuizCommand (Guid QuizId) : IRequest<bool> {

    }
}
