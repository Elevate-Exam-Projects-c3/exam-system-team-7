using MediatR;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

// The RefreshToken slice's namespace shadows the entity name inside
// Features.Identity, so the entity gets an alias here.
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;

namespace exam_system.Features.Identity.Login.Handlers;

// Inserts one refresh row; the token value comes from the orchestrator.
public class CreateRefreshTokenCommandHandler
    : IRequestHandler<CreateRefreshTokenCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<RefreshTokenEntity> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRefreshTokenCommandHandler(
        IGenericRepository<RefreshTokenEntity> refreshTokens,
        IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshTokenEntity
        {
            UserId = request.UserId,
            Token = request.Token,
            ExpiresAt = request.ExpiresAt,
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(refreshToken.Id);
    }
}
