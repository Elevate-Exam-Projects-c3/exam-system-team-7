using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Orchestrators {
    public record CreateQuizOrchestrator (Guid DiplomaId,
        string Title,
        string? Instructions,
        int DurationMinutes,
        int? PassScore,
        int? MaxAttempts,
        DateTime StartDate,
        DateTime EndDate ) : IRequest<RequestResponse<bool>> {
     }
}
