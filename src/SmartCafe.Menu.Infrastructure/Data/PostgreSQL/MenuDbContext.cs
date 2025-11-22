using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

public class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options)
{
    public DbSet<Cafe> Cafes => Set<Cafe>();
    public DbSet<Domain.Entities.Menu> Menus => Set<Domain.Entities.Menu>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItemCategory> MenuItemCategories => Set<MenuItemCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MenuDbContext).Assembly);

        // Global configuration for all DateTime properties to use UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");
                }
            }
        }

        // Seed default categories
        var seedTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var vegetarianId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
        var spicyId = Guid.Parse("550e8400-e29b-41d4-a716-446655440002");

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = vegetarianId, Name = "Vegetarian", IsDefault = true, CreatedAt = seedTime },
            new Category { Id = spicyId, Name = "Spicy", IsDefault = true, CreatedAt = seedTime }
        );
    }
}
