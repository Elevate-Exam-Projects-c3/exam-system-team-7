using exam_system.Dtos.Quizes;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Commands {
    public record CreateQuizCommand(CreateQuizDto CreateQuiz ) : IRequest<Guid>;

}
