using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.RefreshToken.Commands;

// Marks the consumed refresh row and records its replacement.
public record ConsumeRefreshTokenCommand(Guid TokenId, string ReplacedByToken)
    : IRequest<RequestResponse<Guid>>;
