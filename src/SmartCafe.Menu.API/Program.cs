using Azure.Identity;
using Scalar.AspNetCore;
using Serilog;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.API.Middleware;
using SmartCafe.Menu.Application.DependencyInjection;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.Services;
using SmartCafe.Menu.Infrastructure.DependencyInjection;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Menu API application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    // Configure secrets (Key Vault in production, User Secrets in dev)
    if (!builder.Environment.IsDevelopment())
    {
        var keyVaultUri = builder.Configuration["KeyVault:Uri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }

    // Domain services
    builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

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

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.MapRoutes();

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
