using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Forum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ForumDbContext db, IConfiguration config) : ControllerBase
{
    private readonly PasswordHasher<User> _hasher = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (db.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
            return BadRequest("Username or email already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _hasher.HashPassword(null!, request.Password),
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = db.Users.SingleOrDefault(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail);
        if (user == null)
            return Unauthorized();

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result != PasswordVerificationResult.Success)
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var jwtConfig = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtConfig["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}
