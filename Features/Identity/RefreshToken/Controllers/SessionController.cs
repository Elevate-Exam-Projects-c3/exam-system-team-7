using MediatR;
using Microsoft.AspNetCore.Mvc;
using exam_system.Features.Identity.RefreshToken.Orchestrators;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.RefreshToken.Controllers;

// Refresh endpoint; the refresh token travels only in the httpOnly
// cookie, never in a request body.
[ApiController]
[Route("api/auth")]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtOptions _jwtOptions;

    public SessionController(IMediator mediator, Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions.Value;
    }

    // POST /api/auth/refresh-token — rotates the cookie's refresh token
    // and renews the access token.
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(_jwtOptions.CookieName, out var refreshToken);

        var response = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

        if (response.Success && response.Data is not null)
        {
            // The rotated token replaces the cookie value in the same response.
            Response.SetRefreshTokenCookie(_jwtOptions, response.Data.RefreshToken);
        }

        return StatusCode(response.StatusCode, response);
    }
}
