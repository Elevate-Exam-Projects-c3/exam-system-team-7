using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Orchestrators;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Creates one pending ApplicationUser; 409 if the email already exists.
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IGenericRepository<ApplicationUser> users, IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Plain equality is sargable and case-insensitive (CI collation).
        var emailTaken = await _users
            .Get(u => u.Email == request.Email)
            .AnyAsync(cancellationToken);

        if (emailTaken)
        {
            return RequestResponse<Guid>.Fail(
                "Email already registered.",
                409,
                new Dictionary<string, string[]>
                {
                    [nameof(RegisterUserCommand.Email)] = new[] { "Email already registered." }
                });
        }

        // Role/status defaults come from the entity.
        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.PasswordHash
        };

        await _users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(user.Id);
    }
}
