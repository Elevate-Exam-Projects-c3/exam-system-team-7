using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// EXAM-102: the Handler owns the BUSINESS rules — the Validator only owns
// shape/format rules (team convention). First business rule: case-insensitive
// email uniqueness -> 409 "Email already registered".
// EXAM-103 adds the rest of the flow: hash the password, create the pending
// user, and trigger the OTP email.
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<ApplicationUser> _users;

    public RegisterUserCommandHandler(IGenericRepository<ApplicationUser> users)
    {
        _users = users;
    }

    public async Task<RequestResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize BEFORE comparing (and later before saving): trim spaces +
        // lowercase, so "Admin@ExamSystem.com " and "admin@examsystem.com"
        // become the exact same value.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Application-layer uniqueness check. ToLower() on the column is
        // translated by EF Core to LOWER(Email) in SQL, so the comparison is
        // case-insensitive no matter which collation the database uses.
        // The global soft-delete query filter is applied automatically,
        // so a deleted user does not block a new registration.
        var emailTaken = await _users
            .Get(u => u.Email.ToLower() == normalizedEmail)
            .AnyAsync(cancellationToken);

        if (emailTaken)
        {
            // 409 Conflict with a SPECIFIC message and a field-mapped error,
            // exactly as the user story requires (not a generic error).
            return RequestResponse<Guid>.Fail(
                "Email already registered.",
                409,
                new Dictionary<string, string[]>
                {
                    [nameof(RegisterUserCommand.Email)] = new[] { "Email already registered." }
                });
        }

        // TEMPORARY success — proves the business rule ran after validation.
        // EXAM-103 replaces this with: hash the password via IPasswordHasher,
        // create the pending user (Role=Student, AccountStatus=Pending), send OTP.
        return RequestResponse<Guid>.Ok(Guid.NewGuid(), "Email is available - Handler reached.");
    }
}
