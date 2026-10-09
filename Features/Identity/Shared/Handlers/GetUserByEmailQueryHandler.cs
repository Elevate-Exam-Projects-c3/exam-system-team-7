using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Shared.Dtos;
using exam_system.Features.Identity.Shared.Queries;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Shared.Handlers;

// Email lookup; maps the row to the DTO.
public class GetUserByEmailQueryHandler
    : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IGenericRepository<ApplicationUser> _users;

    public GetUserByEmailQueryHandler(IGenericRepository<ApplicationUser> users)
    {
        _users = users;
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _users
            .Get(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? null
            : new UserDto(
                user.Id,
                user.Email,
                user.PasswordHash,
                user.Role,
                user.AccountStatus,
                user.EmailConfirmed,
                user.LockoutEnd);
    }
}
