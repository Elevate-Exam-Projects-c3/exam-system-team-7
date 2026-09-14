using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Login.Orchestrators;

// Login request; handled by LoginUserOrchestrator.
public record LoginUserCommand(string Email, string Password)
    : IRequest<RequestResponse<LoginResponse>>;
