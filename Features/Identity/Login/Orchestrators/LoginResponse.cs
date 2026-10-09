namespace exam_system.Features.Identity.Login.Orchestrators;

// Login success payload: access token for the body, refresh token for the cookie.
public record LoginResponse(string AccessToken, string RefreshToken);
