using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// The flow-entry request for the registration use case (EXAM-103).
// Placement rule (team, 2026-09-10): this request is handled by the
// ORCHESTRATOR — the multi-step flow coordinator — so it lives beside it in
// Orchestrators/. It is different in kind from the single-object commands in
// Commands/ (CreateUserCommand, CreateEmailVerificationOtpCommand), which
// each mutate ONE object. Naming rule: a flow request is NEVER handled by a
// same-name plain handler — only by an Orchestrator with "Orchestrator" in
// the class name.
public record RegisterUserCommand(string FullName, string Email, string Password)
    : IRequest<RequestResponse<Guid>>;
