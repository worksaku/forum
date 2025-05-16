using System.Linq.Expressions;
using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public class ForumDbContext(DbContextOptions<ForumDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
        // Apply global IsDeleted = false filter to all BaseModel-derived entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseModel.IsDeleted));
                var filter = Expression.Lambda(
                    Expression.Equal(prop, Expression.Constant(false)),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseModel>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDateTimeUtc = DateTime.UtcNow;
                    entry.Entity.ModifiedDateTimeUtc = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDateTimeUtc = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
