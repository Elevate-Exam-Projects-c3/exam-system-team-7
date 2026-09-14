using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Login.Handlers;

// Single responsibility: ONE ApplicationUser's FailedLoginAttempts = 0.
public class ResetFailedLoginAttemptsCommandHandler
    : IRequestHandler<ResetFailedLoginAttemptsCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;

    public ResetFailedLoginAttemptsCommandHandler(
        IGenericRepository<ApplicationUser> users,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(ResetFailedLoginAttemptsCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User {request.UserId} was not found.");

        user.FailedLoginAttempts = 0;
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(user.Id);
    }
}
