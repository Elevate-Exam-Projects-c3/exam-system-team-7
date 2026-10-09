using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// Clears one ApplicationUser's failure streak on a correct password.
public record ResetFailedLoginAttemptsCommand(Guid UserId)
    : IRequest<RequestResponse<Guid>>;
