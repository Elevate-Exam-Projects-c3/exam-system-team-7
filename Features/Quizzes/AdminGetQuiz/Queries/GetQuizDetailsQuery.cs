using exam_system.Dtos.Quizes;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminGetQuiz.Queries {
    public record GetQuizDetailsQuery (Guid QuizId) : IRequest<RequestResponse<QuizDetailsDto>> {
    }
}
