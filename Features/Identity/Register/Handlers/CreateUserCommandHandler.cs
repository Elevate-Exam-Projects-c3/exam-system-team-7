using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Orchestrators;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Single responsibility: mutate the Users table — one object, one state
// change (bootcamp CQRS rule). The business rule owned by this aggregate:
// the email must be unique (case-insensitive). The Orchestrator wraps this
// Command inside its transaction.
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
        // Sargable plain equality: case-insensitive via the DB collation
        // (SQL_Latin1_General_CP1_CI_AS), index seek on IX_AspNetUsers_Email.
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

        // Role=Student, AccountStatus=Pending, EmailConfirmed=false come from
        // the entity defaults (EXAM-101). The Id is generated in memory by
        // BaseEntity, so the Orchestrator can use it before saving.
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
