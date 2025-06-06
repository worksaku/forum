using Forum.Data;
using Microsoft.EntityFrameworkCore;

namespace Forum.IntegrationTests;

public static class TestDbCleaner
{
    public static async Task ClearDatabaseAsync(ForumDbContext db)
    {
        await db.Posts.IgnoreQueryFilters().ExecuteDeleteAsync();
        await db.Users.IgnoreQueryFilters().ExecuteDeleteAsync();
    }
}