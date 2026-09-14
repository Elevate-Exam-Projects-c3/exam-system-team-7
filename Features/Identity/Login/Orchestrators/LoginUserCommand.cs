using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Orchestrators;

// EXAM-107 + EXAM-108: the flow-entry request for login. Handled by the
// ORCHESTRATOR (convention #10) — the flow verifies credentials (EXAM-107)
// AND issues the token pair (EXAM-108: access token + a RefreshTokens row as
// a second aggregate inside one transaction).
public record LoginUserCommand(string Email, string Password)
    : IRequest<RequestResponse<LoginResponse>>;
