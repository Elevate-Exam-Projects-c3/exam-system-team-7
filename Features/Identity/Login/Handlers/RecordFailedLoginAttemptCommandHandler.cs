using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Login.Handlers;

// Single responsibility: ONE ApplicationUser's FailedLoginAttempts +1,
// persisted. The new count travels back so the Orchestrator can decide about
// locking without re-reading the row.
public class RecordFailedLoginAttemptCommandHandler
    : IRequestHandler<RecordFailedLoginAttemptCommand, RequestResponse<int>>
{
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;

    public RecordFailedLoginAttemptCommandHandler(
        IGenericRepository<ApplicationUser> users,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<int>> Handle(RecordFailedLoginAttemptCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User {request.UserId} was not found.");

        user.FailedLoginAttempts++;
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<int>.Ok(user.FailedLoginAttempts);
    }
}
