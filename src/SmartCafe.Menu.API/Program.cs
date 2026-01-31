using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.API.Middleware;
using SmartCafe.Menu.Application.DependencyInjection;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Infrastructure.DependencyInjection;
using SmartCafe.Menu.Shared.Providers;
using SmartCafe.Menu.Shared.Providers.Abstractions;

var testingEnv = "Testing";
var frontendPolicyName = "FrontEnd";

// https://github.com/serilog/serilog-aspnetcore/issues/289
// There is a problem with using Serilog's "CreateBootstrapLogger" when trying to initialize a web host.
// Multiple hosts may be created in parallel during integration tests, which can lead to conflicts when initializing and disposing the shared logger instance.
var isTesting = args.Any(arg => arg.Contains($"environment={testingEnv}", StringComparison.OrdinalIgnoreCase))
    || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == testingEnv;

var loggerConfiguration = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console();

Log.Logger = isTesting
    ? loggerConfiguration.CreateLogger()
    : loggerConfiguration.CreateBootstrapLogger();

try
{
    Log.Information("Starting Menu API application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    var isUpperEnvironment = builder.Environment.IsProduction()
        || builder.Environment.IsStaging();

    // Configure secrets (Key Vault in production, User Secrets in dev)
    if (isUpperEnvironment)
    {
        var keyVaultUri = builder.Configuration["KeyVault:Uri"];

        if (string.IsNullOrEmpty(keyVaultUri))
            throw new InvalidOperationException("Key Vault URI is not configured.");

        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }

    // Configure CORS
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<Dictionary<string, string>>();

    if (allowedOrigins?.Count > 0)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(frontendPolicyName, policy =>
            {
                policy.WithOrigins([.. allowedOrigins.Values])
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    // Domain services
    builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    builder.Services.AddSingleton<IGuidIdProvider, GuidIdProvider>();

    // Application and Infrastructure services
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplicationLayer();

    // Add API services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();
    builder.Services.AddProblemDetails();
    builder.AddServiceDefaults();

    var app = builder.Build();

    // Configure middleware pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (isUpperEnvironment)
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseCors(frontendPolicyName);

    if (!isUpperEnvironment)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.MapRoutes();

    // Apply database migrations at startup
    // TODO: Consider moving to a more robust migration strategy for production scenarios
    // e.g., manual migrations via CI/CD pipeline or a dedicated migrator service
    // Consider database is vnet restricted
    using var scope = app.Services.CreateScope();
    using var dbContext = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
    await dbContext.Database.MigrateAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program accessible for testing
public partial class Program { }
