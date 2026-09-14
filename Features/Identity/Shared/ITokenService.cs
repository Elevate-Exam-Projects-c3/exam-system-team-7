namespace exam_system.Features.Identity.Shared;

// Abstraction over token issuance (Dependency Inversion + Strategy pattern).
// The Login orchestrator depends on this interface — never on the JWT
// library — so the signing algorithm or token format can be replaced in one
// place without touching any feature code. EXAM-5 (refresh) will call
// GenerateAccessToken again for rotation.
public interface ITokenService
{
    // Signed JWT carrying: sub (user id), role (Student/Admin), jti (unique id).
    // Expires after JwtOptions.ExpiryMinutes (story: 15 minutes).
    string GenerateAccessToken(Guid userId, string role);
}
