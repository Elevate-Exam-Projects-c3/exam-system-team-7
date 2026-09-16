using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.RefreshToken.Dtos;
using exam_system.Features.Identity.RefreshToken.Queries;
using exam_system.Persistence.DataAccess;

// The RefreshToken slice's namespace shadows the entity name inside
// Features.Identity, so the entity gets an alias here.
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;

namespace exam_system.Features.Identity.RefreshToken.Handlers;

// Looks up the row by token and maps it to the DTO.
public class GetRefreshTokenByTokenQueryHandler
    : IRequestHandler<GetRefreshTokenByTokenQuery, RefreshTokenDto?>
{
    private readonly IGenericRepository<RefreshTokenEntity> _refreshTokens;

    public GetRefreshTokenByTokenQueryHandler(IGenericRepository<RefreshTokenEntity> refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task<RefreshTokenDto?> Handle(GetRefreshTokenByTokenQuery request, CancellationToken cancellationToken)
    {
        var row = await _refreshTokens
            .Get(r => r.Token == request.Token)
            .Include(r => r.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (row?.User is null)
        {
            return null;
        }

        return new RefreshTokenDto(
            row.Id,
            row.UserId,
            row.User.Role.ToString(),
            row.IsUsed,
            row.IsRevoked,
            row.ExpiresAt);
    }
}
