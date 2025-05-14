using Forum.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Forum.IntegrationTests;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    private readonly IServiceScope _scope;
    protected readonly ForumDbContext Db;
    protected readonly CustomWebAppFactory Factory;

    public TestBase(CustomWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        Db = _scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    }

    public async Task InitializeAsync()
    {
        await Db.Database.EnsureDeletedAsync();
        await Db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }
}
