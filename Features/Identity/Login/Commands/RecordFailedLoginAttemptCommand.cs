using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// Increments one ApplicationUser's FailedLoginAttempts.
public record RecordFailedLoginAttemptCommand(Guid UserId)
    : IRequest<RequestResponse<int>>;
