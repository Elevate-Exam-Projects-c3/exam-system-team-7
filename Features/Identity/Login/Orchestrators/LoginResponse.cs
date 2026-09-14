namespace exam_system.Features.Identity.Login.Orchestrators;

// The login success payload. It carries BOTH tokens to the controller:
//   * AccessToken — goes in the response BODY (the client sends it back in
//     the Authorization header on every request).
//   * RefreshToken — the CONTROLLER turns this value into the httpOnly
//     cookie (HTTP concern lives with HTTP, not inside the flow). The value
//     itself is never logged and never returned anywhere else.
public record LoginResponse(string AccessToken, string RefreshToken);
