using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.CheckRemainingTime.Commands {
    public record TimeoutAttemptCommand (Guid attemptId) : IRequest<RequestResponse<bool>> {

    }    
}
