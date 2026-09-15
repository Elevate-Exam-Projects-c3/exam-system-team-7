using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.GetOldAttempt.Queries {
    public record CheckExistingAttemptQuery (Guid StudentId,Guid QuizId) :IRequest<RequestResponse<Guid?>> {
    }
}
