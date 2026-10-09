using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Logout.Commands;

// Revokes ONE RefreshTokens row by its token value.
public record RevokeRefreshTokenCommand(string Token)
    : IRequest<RequestResponse<bool>>;
