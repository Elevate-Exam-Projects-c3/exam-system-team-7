namespace exam_system.Features.Identity.Shared;

// Strongly-typed JWT settings bound from the "Jwt" config section
// (same pattern as SmtpOptions). All times are server-side values — the
// client mirrors them but the server enforces them.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 15;      // story rule: access token TTL
    public int RefreshTokenDays { get; set; } = 7;    // story rule: refresh token TTL
    public string CookieName { get; set; } = "refreshToken";
}
