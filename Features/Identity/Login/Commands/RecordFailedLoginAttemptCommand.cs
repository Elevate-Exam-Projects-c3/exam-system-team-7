using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// EXAM-107: single-object mutation — ONE ApplicationUser's FailedLoginAttempts +1.
// Mechanics only: the "lock once the count reaches the limit" DECISION is the
// Orchestrator's (it sends LockUserAccountCommand based on the returned count).
public record RecordFailedLoginAttemptCommand(Guid UserId)
    : IRequest<RequestResponse<int>>;
