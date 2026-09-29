using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Account-scoped mutation: every reset token still waiting for Step 3 dies
// the moment a new code is issued (at most ONE live token per account).
public record InvalidateStaleResetTokensCommand(string Email)
    : IRequest<RequestResponse<int>>;
