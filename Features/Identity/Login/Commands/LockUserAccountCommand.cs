using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// Sets one ApplicationUser's LockoutEnd.
public record LockUserAccountCommand(Guid UserId, int LockoutMinutes)
    : IRequest<RequestResponse<Guid>>;
