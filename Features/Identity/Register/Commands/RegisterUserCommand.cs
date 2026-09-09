using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// EXAM-102/EXAM-103: the write request for creating a new Student account.
// It carries only raw input — hashing happens later, inside the Handler.
public record RegisterUserCommand(string FullName, string Email, string Password)
    : IRequest<RequestResponse<Guid>>;
