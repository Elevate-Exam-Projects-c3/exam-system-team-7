using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.RefreshToken.Commands;

// Marks one RefreshTokens row as used and records the token that replaced it.
public record ConsumeRefreshTokenCommand(Guid TokenId, string ReplacedByToken)
    : IRequest<RequestResponse<Guid>>;
