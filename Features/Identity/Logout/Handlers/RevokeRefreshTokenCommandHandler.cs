using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.Logout.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

// The RefreshToken slice's namespace shadows the entity name inside
// Features.Identity, so the entity gets an alias here.
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;

namespace exam_system.Features.Identity.Logout.Handlers;

// Revokes one refresh row; an unknown or revoked token is a no-op.
public class RevokeRefreshTokenCommandHandler
    : IRequestHandler<RevokeRefreshTokenCommand, RequestResponse<bool>>
{
    private readonly IGenericRepository<RefreshTokenEntity> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRefreshTokenCommandHandler(
        IGenericRepository<RefreshTokenEntity> refreshTokens,
        IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokens
            .Get(r => r.Token == request.Token)
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null || token.IsRevoked)
        {
            return RequestResponse<bool>.Ok(false);
        }

        token.IsRevoked = true;
        _refreshTokens.Update(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<bool>.Ok(true);
    }
}
