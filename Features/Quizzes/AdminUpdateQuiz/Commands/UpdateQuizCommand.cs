using exam_system.Dtos.Quizes;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Commands {
    public record UpdateQuizCommand(Guid QuizId,UpdateQuizDto UpdateQuiz) : IRequest<Guid> {
    }
}
