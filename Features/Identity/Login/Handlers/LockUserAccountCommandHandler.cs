using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Login.Handlers;

// Single responsibility: ONE ApplicationUser's LockoutEnd = now + LockoutMinutes.
public class LockUserAccountCommandHandler
    : IRequestHandler<LockUserAccountCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;

    public LockUserAccountCommandHandler(
        IGenericRepository<ApplicationUser> users,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(LockUserAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User {request.UserId} was not found.");

        user.LockoutEnd = DateTime.UtcNow.AddMinutes(request.LockoutMinutes);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(user.Id);
    }
}
