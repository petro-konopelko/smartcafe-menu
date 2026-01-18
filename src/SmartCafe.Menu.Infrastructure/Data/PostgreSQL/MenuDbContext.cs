using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

public class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options)
{
    public DbSet<Cafe> Cafes => Set<Cafe>();
    public DbSet<Domain.Entities.Menu> Menus => Set<Domain.Entities.Menu>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

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
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var testCafeId = new Guid("00000000-0000-0000-0000-000000000001");

            var testCafe = await context.Set<Cafe>().FirstOrDefaultAsync(b => b.Id == testCafeId, cancellationToken);
            if (testCafe == null)
            {
                testCafe = Cafe.Create(testCafeId, "Test Cafe", DateTime.UtcNow).EnsureValue();

                context.Set<Cafe>().Add(testCafe);

                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }
}
