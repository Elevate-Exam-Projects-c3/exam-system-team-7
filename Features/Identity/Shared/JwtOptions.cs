namespace exam_system.Features.Identity.Shared;

// JWT settings bound from the "Jwt" config section.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 15;      // access token TTL
    public int RefreshTokenDays { get; set; } = 7;    // refresh token TTL
    public string CookieName { get; set; } = "refreshToken";
}
