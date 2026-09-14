using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using exam_system.Features.Identity.Login.Orchestrators;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.Login.Controllers;

// Thin controller for the LOGIN slice (EXAM-107 + EXAM-108). It lives in its
// own feature slice per the team convention (convention #3) — the register
// slice's AuthController owns register/verify-otp/resend-otp, and this
// controller owns the login endpoint while sharing the same "api/auth" route
// prefix. It injects IMediator and forwards the request; the ONLY extra job
// it has (EXAM-108) is turning the response's refresh token into an httpOnly
// cookie — an HTTP concern that belongs with HTTP, not inside the flow.
[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtOptions _jwtOptions;

    public LoginController(IMediator mediator, Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions.Value;
    }

    // POST /api/auth/login — body: { email, password }
    // Success: 200 — body carries the JWT access token (15-min TTL); the
    // refresh token (7-day TTL) is delivered as an httpOnly cookie.
    // EXAM-109: rate-limited to 10 requests/minute per IP (429 + Retry-After).
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        if (response.Success && response.Data is not null)
        {
            // httpOnly: JavaScript CANNOT read it (XSS cannot steal the
            // 7-day token). SameSite=Lax: sent on same-site requests (and
            // top-level navigations) — enough for our SPA + Swagger. Secure
            // would be required in production HTTPS; flagged in the HANDOFF.
            Response.Cookies.Append(
                _jwtOptions.CookieName,
                response.Data.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // DEV ONLY — production must be true (HTTPS)
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
                });
        }

        return StatusCode(response.StatusCode, response);
    }
}
