using MediatR;
using exam_system.Features.Identity.RefreshToken.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

// The RefreshToken slice's namespace shadows the entity name inside
// Features.Identity, so the entity gets an alias here.
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;

namespace exam_system.Features.Identity.RefreshToken.Handlers;

// Flips ONE RefreshTokens row to IsUsed=true and records its replacement,
// leaving the rotation chain readable in the database.
public class ConsumeRefreshTokenCommandHandler
    : IRequestHandler<ConsumeRefreshTokenCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<RefreshTokenEntity> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeRefreshTokenCommandHandler(
        IGenericRepository<RefreshTokenEntity> refreshTokens,
        IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(ConsumeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokens.GetByIdAsync(request.TokenId)
            ?? throw new InvalidOperationException($"RefreshTokens row {request.TokenId} was not found.");

        token.IsUsed = true;
        token.ReplacedByToken = request.ReplacedByToken;
        _refreshTokens.Update(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(token.Id);
    }
}
