using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PortfolioBackend.Models.DTOs;

namespace PortfolioBackend.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOkAndAssignsOnlyGastRole()
    {
        // Arrange
        var client = _factory.CreateCookieClient();
        var registerDto = new RegisterDto
        {
            Email = "nieuwegast@test.local",
            Password = "Welkom123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var createdUser = await userManager.FindByEmailAsync("nieuwegast@test.local");

        Assert.NotNull(createdUser);
        var roles = await userManager.GetRolesAsync(createdUser);
        Assert.Single(roles);
        Assert.Contains("Gast", roles);
        Assert.DoesNotContain("Editor", roles);
        Assert.DoesNotContain("Admin", roles);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkAndSetsAuthCookie()
    {
        // Arrange
        var client = _factory.CreateCookieClient();
        var loginDto = new LoginDto
        {
            Email = "editor@test.local",
            Password = "Test123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        Assert.Contains(cookies, c => c.Contains(".AspNetCore.Identity.Application"));
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateCookieClient();
        var loginDto = new LoginDto
        {
            Email = "editor@test.local",
            Password = "VerkeerdWachtwoord123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}