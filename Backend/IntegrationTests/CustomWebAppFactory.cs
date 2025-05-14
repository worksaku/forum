using Forum;
using Forum.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.IntegrationTests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ForumDbContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ForumDbContext>(options =>
            {
                options.UseSqlServer(
                    "Server=localhost,1435;Database=ForumTestDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
                );
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
