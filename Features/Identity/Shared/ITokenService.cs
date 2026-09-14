namespace exam_system.Features.Identity.Shared;

// JWT access token issuance; implementation: JwtTokenService.
public interface ITokenService
{
    // Signed JWT with sub / role / jti claims.
    string GenerateAccessToken(Guid userId, string role);
}
