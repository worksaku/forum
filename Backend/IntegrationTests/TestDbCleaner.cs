using Forum.Data;
using Microsoft.EntityFrameworkCore;

namespace Forum.IntegrationTests;

public static class TestDbCleaner
{
    public static async Task ClearDatabaseAsync(ForumDbContext db)
    {
        // Delete in reverse FK order if needed
        await db.Posts.ExecuteDeleteAsync();
        await db.Users.ExecuteDeleteAsync();
    }
}