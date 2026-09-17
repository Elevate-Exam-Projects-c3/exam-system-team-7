using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminManageQuestions.Queries {
    public record ValidateQuestionForQuizQuery (Guid QuizId, Guid QuestionId)  : IRequest<RequestResponse<bool>>{
    }
}
