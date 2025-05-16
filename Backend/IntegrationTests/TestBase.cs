using Forum.Data;
using Xunit;

namespace Forum.IntegrationTests;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly ForumDbContext Db;
    protected readonly CustomWebAppFactory Factory;
    private readonly IServiceScope _scope;

    public TestBase(CustomWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    }

    public async Task InitializeAsync()
    {
        // Ensure database is created and clean before each test
        await Db.Database.EnsureCreatedAsync();
        await TestDbCleaner.ClearDatabaseAsync(Db);
    }

    public async Task DisposeAsync()
    {
        await Db.DisposeAsync();
        _scope.Dispose();
    }
}