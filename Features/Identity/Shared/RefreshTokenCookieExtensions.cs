using Microsoft.AspNetCore.Http;

namespace exam_system.Features.Identity.Shared;

// Writes the refresh token cookie; login and refresh share this shape.
public static class RefreshTokenCookieExtensions
{
    public static void SetRefreshTokenCookie(this HttpResponse response, JwtOptions jwtOptions, string token)
    {
        response.Cookies.Append(jwtOptions.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // dev only; must be true behind HTTPS
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.RefreshTokenDays)
        });
    }
}
