using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// Inserts one RefreshTokens row for the logged-in user.
public record CreateRefreshTokenCommand(Guid UserId, string Token, DateTime ExpiresAt)
    : IRequest<RequestResponse<Guid>>;
