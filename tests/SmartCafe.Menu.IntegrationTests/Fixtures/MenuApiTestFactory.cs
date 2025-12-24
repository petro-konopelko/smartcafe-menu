using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Infrastructure.BlobStorage;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.IntegrationTests.Mocks;

namespace SmartCafe.Menu.IntegrationTests.Fixtures;

public sealed class MenuApiTestFactory(string connectionString) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = string.IsNullOrWhiteSpace(connectionString)
        ? throw new ArgumentException("Connection string is required.", nameof(connectionString))
        : connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<MenuDbContext>>();
            services.RemoveAll<MenuDbContext>();

            services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(_connectionString));

            // Disable external integrations for tests
            services.RemoveAll<IDomainEventDispatcher>();
            services.AddScoped<IDomainEventDispatcher, NoOpDomainEventDispatcher>();

            services.RemoveAll<AzureBlobStorageService>();
            services.AddSingleton<IImageStorageService, NoOpImageStorageService>();
        });
    }
}
