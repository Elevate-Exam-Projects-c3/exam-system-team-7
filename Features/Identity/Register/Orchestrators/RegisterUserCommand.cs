using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// Registration request; handled by RegisterUserOrchestrator.
public record RegisterUserCommand(string FullName, string Email, string Password)
    : IRequest<RequestResponse<Guid>>;
