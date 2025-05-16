using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Forum.IntegrationTests;

public class AuthApiTest(CustomWebAppFactory factory) : TestBase(factory), IClassFixture<CustomWebAppFactory>
{

    #region Register

    [Fact]
    public async Task Register_ReturnsOk()
    {
        // Arrange
        var request = new RegisterRequest("testuser", "foobar@email.com", "password123");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var user = await Db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        Assert.NotNull(user);
        Assert.Equal(request.Username, user.Username);
        Assert.Equal(request.Email, user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEqual(request.Password, user.PasswordHash);
    }

    #endregion
    #region Login

    [Fact]
    public async Task Login_ReturnsOk()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "foobar@example.com",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "password123"),
        };
        Db.Users.Add(user);
        await Db.SaveChangesAsync();

        // Act
        var request = new LoginRequest("testuser", "password123");
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);
        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = json?["token"];
        var userId = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var roles = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
        var expiration = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo;
        var issuedAt = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidFrom;
        var issuer = new JwtSecurityTokenHandler().ReadJwtToken(token).Issuer;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(token);
        Assert.NotNull(userId);
        Assert.Equal(user.Username, username);
        Assert.Equal(user.Email, email);
        Assert.NotNull(roles);
        Assert.NotNull(issuer);
    }

    #endregion
}
