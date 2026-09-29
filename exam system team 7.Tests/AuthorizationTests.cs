using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace exam_system.Tests;

// EXAM-115: calls every protected endpoint with Admin and Student tokens
// and asserts the expected 200/403 outcome for each.
public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthorizationTests(WebApplicationFactory<Program> factory)
    {
        // WebApplicationFactory defaults to the Production environment, which
        // has no "Jwt" section — force Development so the real config loads.
        _factory = factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment(Microsoft.Extensions.Hosting.Environments.Development));
    }

    private async Task<string> LoginAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.Data.AccessToken;
    }

    private HttpClient ClientWithToken(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // Every protected endpoint with the outcome expected from each role.
    // Admin-only endpoints: Admin → allowed (any non-401/403 status), Student → 403. Student-only: the reverse.
    public static TheoryData<string, HttpMethod, string?, HttpStatusCode, HttpStatusCode> ProtectedEndpoints => new()
    {
        // { url, method, body, admin outcome, student outcome }
        // POST with an empty body passes authorization (Admin) then fails validation → 400, not 403.
        { "/api/admin/diplomas", HttpMethod.Post, "{}", HttpStatusCode.BadRequest, HttpStatusCode.Forbidden },
        { "/api/admin/diplomas", HttpMethod.Get, null, HttpStatusCode.Forbidden, HttpStatusCode.OK },
        { "/api/admins", HttpMethod.Get, null, HttpStatusCode.OK, HttpStatusCode.Forbidden },
        { "/api/studentsdashboard?StudentId=AAAAAAAA-1111-1111-1111-AAAAAAAAAAAA", HttpMethod.Get, null, HttpStatusCode.Forbidden, HttpStatusCode.OK },
    };

    [Theory]
    [MemberData(nameof(ProtectedEndpoints))]
    public async Task ProtectedEndpoint_ReturnsExpectedStatus_ForEachRole(
        string url, HttpMethod method, string? body,
        HttpStatusCode adminExpected, HttpStatusCode studentExpected)
    {
        var adminToken = await LoginAsync("admin@examsystem.com", "Admin@123456");
        var studentToken = await LoginAsync("john.doe@student.com", "Student@123456");

        var adminResponse = await SendAsync(ClientWithToken(adminToken), method, url, body);
        Assert.Equal(adminExpected, adminResponse.StatusCode);

        var studentResponse = await SendAsync(ClientWithToken(studentToken), method, url, body);
        Assert.Equal(studentExpected, studentResponse.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/admins");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithSeedAdmin_ReturnsTokenWithAdminRole()
    {
        var token = await LoginAsync("admin@examsystem.com", "Admin@123456");

        // Validate with the SAME key the app issued the token with (read from config — single source of truth).
        var config = _factory.Services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        var key = config["Jwt:Key"]!;

        var principal = new System.Security.Claims.ClaimsPrincipal(
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                .ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(key)),
                }, out _));

        Assert.Equal("Admin", principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value);
    }

    private static async Task<HttpResponseMessage> SendAsync(
        HttpClient client, HttpMethod method, string url, string? body)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        return await client.SendAsync(request);
    }

    private record LoginResponse(LoginData Data);
    private record LoginData(string AccessToken, string RefreshToken);
}
