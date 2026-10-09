using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Single-object mutation: clear ONE row's reset-token columns — the AC's
// "the reset token is invalidated immediately".
public record InvalidateResetTokenCommand(Guid OtpId)
    : IRequest<RequestResponse>;
