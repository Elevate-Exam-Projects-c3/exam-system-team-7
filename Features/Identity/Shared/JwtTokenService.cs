using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace exam_system.Features.Identity.Shared;

// The only place in the app that builds JWTs (same role BCryptPasswordHasher
// plays for bcrypt). Reads the settings from JwtOptions; signs with
// HMAC-SHA256. Stateless and thread-safe -> registered as a Singleton.
public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateAccessToken(Guid userId, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var claims = new[]
        {
            // "sub" — who the token belongs to (the user id).
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            // "role" — consumed by the EXAM-8 authorization policies.
            new Claim(JwtRegisteredClaimNames.UniqueName, role),
            // "jti" — a unique id per token (useful for EXAM-5 logout/refresh).
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
