using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// Creates one pending ApplicationUser with the already-hashed password.
public record CreateUserCommand(string FullName, string Email, string PasswordHash)
    : IRequest<RequestResponse<Guid>>;
