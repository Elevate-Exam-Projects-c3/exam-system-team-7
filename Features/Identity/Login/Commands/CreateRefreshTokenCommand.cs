using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Commands;

// EXAM-108: single-object mutation — insert ONE RefreshTokens row for the
// freshly logged-in user. The Orchestrator decides WHEN and WITH WHAT TOKEN
// VALUE; this command only persists it. Per the team decision
// (2026-09-13): one refresh token per user — a new login revokes any
// previous one, so this command is sent after that revocation.
public record CreateRefreshTokenCommand(Guid UserId, string Token, DateTime ExpiresAt)
    : IRequest<RequestResponse<Guid>>;
