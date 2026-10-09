using MediatR;
using exam_system.Features.Identity.RefreshToken.Dtos;

namespace exam_system.Features.Identity.RefreshToken.Queries;

// Reads one refresh row for the refresh flow.
public record GetRefreshTokenByTokenQuery(string Token)
    : IRequest<RefreshTokenDto?>;
