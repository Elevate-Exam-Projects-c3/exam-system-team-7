using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Commands {
    public record UpdateQuizCommand(
        Guid QuizId,
        string? Title,
        string? Instructions,
        int? DurationMinutes,
        int? PassScore,
        int? MaxAttempts,
        DateTime? StartDate,
        DateTime? EndDate
    ) : IRequest<RequestResponse<Guid>>;
}

