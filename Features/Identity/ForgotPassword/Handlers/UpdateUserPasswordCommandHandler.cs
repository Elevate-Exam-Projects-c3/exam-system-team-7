using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// ONE ApplicationUser: PasswordHash swap only (lockout columns untouched —
// a 15-minute lockout expires on its own).
public class UpdateUserPasswordCommandHandler
    : IRequestHandler<UpdateUserPasswordCommand, RequestResponse>
{
    private readonly IGenericRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserPasswordCommandHandler(
        IGenericRepository<ApplicationUser> users,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User {request.UserId} was not found.");

        user.PasswordHash = request.PasswordHash;

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse.Ok("Password updated.");
    }
}
