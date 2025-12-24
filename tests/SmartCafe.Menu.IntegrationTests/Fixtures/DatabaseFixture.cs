using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using Testcontainers.PostgreSql;

namespace SmartCafe.Menu.IntegrationTests.Fixtures;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public string ConnectionString { get; private set; } = string.Empty;

    public DatabaseFixture()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("MenusTestDb")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        ConnectionString = _dbContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<MenuDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new MenuDbContext(options);
        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
