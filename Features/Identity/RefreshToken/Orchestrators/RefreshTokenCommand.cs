using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.RefreshToken.Orchestrators;

// Flow entry: rotate one refresh token into a fresh access/refresh pair.
// The value arrives from the httpOnly cookie, so there is no request body
// and no validator — the gates live in the orchestrator.
public record RefreshTokenCommand(string? RefreshToken)
    : IRequest<RequestResponse<RefreshResponse>>;
