using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using Testcontainers.PostgreSql;

namespace SmartCafe.Menu.IntegrationTests.Fixtures;

public class MenuApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("MenusTestDb")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove all DbContext registrations safely
            services.RemoveAll<DbContextOptions<MenuDbContext>>();
            services.RemoveAll<MenuDbContext>();

            // Register test DB
            services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();

        await db.Database.MigrateAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
