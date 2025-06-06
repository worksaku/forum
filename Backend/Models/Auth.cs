namespace Forum.Models;

public record LoginRequest(string UsernameOrEmail, string Password);

public record RegisterRequest(string Username, string Email, string Password);
