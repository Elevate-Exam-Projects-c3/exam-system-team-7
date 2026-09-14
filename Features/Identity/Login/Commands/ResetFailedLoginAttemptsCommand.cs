using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// EXAM-107: single-object mutation — clear ONE ApplicationUser's failure
// streak. Sent when the submitted password is CORRECT: a correct password
// ends the consecutive-failure streak even while the account is still pending.
public record ResetFailedLoginAttemptsCommand(Guid UserId)
    : IRequest<RequestResponse<Guid>>;
