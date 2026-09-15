using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.GetOldAttempt.Queries {
    public record CheckMaxAttemptsQuery(Guid StudentId, Guid QuizId, int? MaxAttempts) : IRequest<RequestResponse<bool>> {

    }
}
