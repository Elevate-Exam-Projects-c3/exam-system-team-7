using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Single-object mutation: ONE ApplicationUser gets a new PasswordHash.
public record UpdateUserPasswordCommand(Guid UserId, string PasswordHash)
    : IRequest<RequestResponse>;
