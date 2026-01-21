using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
{
    public MenuDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MenuDbContext>();

        // Use a dummy connection string for design-time (migrations)
        optionsBuilder.UseNpgsql("Host=localhost;Database=smartcafe_menu;Username=postgres;Password=dummy");

        return new MenuDbContext(optionsBuilder.Options);
    }
}
