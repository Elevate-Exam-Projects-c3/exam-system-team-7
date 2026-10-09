using MediatR;
using exam_system.Features.Identity.Shared.Dtos;

namespace exam_system.Features.Identity.Shared.Queries;

// User lookup by email, shared by the Identity flows.
public record GetUserByEmailQuery(string Email)
    : IRequest<UserDto?>;
