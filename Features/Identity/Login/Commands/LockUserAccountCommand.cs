using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// EXAM-107: single-object mutation — set ONE ApplicationUser's LockoutEnd.
// The Orchestrator decides WHEN (the story: five consecutive failures);
// this command only applies the state change.
public record LockUserAccountCommand(Guid UserId, int LockoutMinutes)
    : IRequest<RequestResponse<Guid>>;
