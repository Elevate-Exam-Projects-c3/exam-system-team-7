using exam_system.Dtos.Attempts;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.GetOldAttempt.Queries {
    public record GetOldQuizAttemptQuery (Guid attemptId) : IRequest<RequestResponse<QuizAttemptDto>> {
    }
}
