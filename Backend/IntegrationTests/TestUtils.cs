using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.IntegrationTests.Utils;

public static class TestUtils
{
    public async static Task<User> CreateTestUser(string username = "testuser", string email = "testuser@example.com", string password = "password123", ForumDbContext? db = null)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, password),
        };
        if (db != null)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        return user;
    }

    public static void LoginTestUser(HttpClient client, CustomWebAppFactory factory, User user)
    {
        var token = AuthTestHelper.GenerateJwtToken(user.Id, user.Username, factory.Configuration);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    }

    public async static Task<Post> CreateTestPost(Guid authorId, string? title = null, string? content = null, bool isDeleted = false, ForumDbContext? db = null)
    {
        var post = new Post
        {
            Title = title ?? CreateRandomText(),
            Content = content ?? CreateRandomText(100),
            AuthorId = authorId,
            IsDeleted = isDeleted
        };
        if (db != null)
        {
            db.Posts.Add(post);
            await db.SaveChangesAsync();
        }
        return post;
    }

    private static string CreateRandomText(int length = 20)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }
}