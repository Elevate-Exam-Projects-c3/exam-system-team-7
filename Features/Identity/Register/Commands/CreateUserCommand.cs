using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// EXAM-103: a Command mutates ONE object's state (bootcamp CQRS rule).
// Creating the user is ONE step of the registration flow — the multi-step
// coordination lives in the Orchestrator, never here.
public record CreateUserCommand(string FullName, string Email, string PasswordHash)
    : IRequest<RequestResponse<Guid>>;
