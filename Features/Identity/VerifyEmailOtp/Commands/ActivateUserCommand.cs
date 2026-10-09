using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.VerifyEmailOtp.Commands;

// Single-object mutation: ONE ApplicationUser becomes Active + EmailConfirmed.
public record ActivateUserCommand(Guid UserId)
    : IRequest<RequestResponse<Guid>>;
