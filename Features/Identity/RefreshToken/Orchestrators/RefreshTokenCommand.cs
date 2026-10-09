using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.RefreshToken.Orchestrators;

// Flow entry; the token value arrives from the httpOnly cookie.
public record RefreshTokenCommand(string? RefreshToken)
    : IRequest<RequestResponse<RefreshResponse>>;
