using Forum.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Forum.IntegrationTests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration => _configuration!;
    private IConfiguration? _configuration;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            _configuration = configBuilder.Build();
        });

        builder.ConfigureServices(services =>
        {
            // Remove original DB config
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ForumDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Use a shared SQL Server test container (ensure running)
            services.AddDbContext<ForumDbContext>(options =>
                options.UseSqlServer("Server=localhost,1435;Database=ForumTestDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"));
        });
    }
}