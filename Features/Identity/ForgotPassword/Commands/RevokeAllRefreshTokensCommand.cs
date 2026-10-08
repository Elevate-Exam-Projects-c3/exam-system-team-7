using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// User-scoped mutation: EVERY live RefreshTokens row of ONE user flips to
// IsRevoked=true (the AC's "forcing re-login on every device").
public record RevokeAllRefreshTokensCommand(Guid UserId)
    : IRequest<RequestResponse<int>>;
