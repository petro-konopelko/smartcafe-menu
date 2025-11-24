using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Migrator;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting migrator application");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // Database
    var connectionString = builder.Configuration.GetConnectionString("MenuDb")
        ?? throw new InvalidOperationException("Database connection string 'MenuDb' not configured");

    builder.Services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(connectionString));

    // Migrator Service
    builder.Services.AddScoped<MigratorService>();

    var host = builder.Build();

    // Execute migration
    using var scope = host.Services.CreateScope();
    var migratorService = scope.ServiceProvider.GetRequiredService<MigratorService>();
    await migratorService.ExecuteAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
