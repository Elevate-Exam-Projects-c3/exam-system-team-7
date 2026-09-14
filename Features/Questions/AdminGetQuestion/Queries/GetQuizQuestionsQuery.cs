using exam_system.Dtos.Questions;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.AdminGetQuestion.Queries {
    public record GetQuizQuestionsQuery (Guid QuizId) : IRequest<RequestResponse<List<QuestionDto>>> {
    }
}
