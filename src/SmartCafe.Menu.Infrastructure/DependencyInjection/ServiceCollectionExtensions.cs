using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Infrastructure.BlobStorage;
using SmartCafe.Menu.Infrastructure.Data;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Infrastructure.EventBus;
using SmartCafe.Menu.Infrastructure.Repositories;
using SmartCafe.Menu.Infrastructure.Services;

namespace SmartCafe.Menu.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = BuildPostgreSqlConnectionString(configuration);
        services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(connectionString));

        // Repositories and UoW
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Image processing and storage
        services.AddScoped<IImageProcessor, ImageProcessingService>();

        var blobStorageAccountName = configuration["AzureStorage:AccountName"]
            ?? throw new InvalidOperationException("Azure Storage account name not configured");
        var blobContainerName = configuration["AzureStorage:ContainerName"] ?? "menu-images";
        var blobServiceClient = new BlobServiceClient(
            new Uri($"https://{blobStorageAccountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        services.AddSingleton(sp =>
        {
            var imageProcessor = sp.GetRequiredService<IImageProcessor>();
            return new AzureBlobStorageService(blobServiceClient, blobContainerName, imageProcessor);
        });
        services.AddScoped<IImageStorageService>(sp => sp.GetRequiredService<AzureBlobStorageService>());

        // Service Bus event publisher
        var serviceBusNamespace = configuration["AzureServiceBus:Namespace"]
            ?? throw new InvalidOperationException("Service Bus namespace not configured");
        var serviceBusClient = new ServiceBusClient(
            $"{serviceBusNamespace}.servicebus.windows.net",
            new DefaultAzureCredential());
        var topicName = configuration["AzureServiceBus:TopicName"] ?? "menu-events";
        services.AddSingleton(sp => new ServiceBusEventPublisher(serviceBusClient, topicName));
        services.AddScoped<IEventPublisher>(sp => sp.GetRequiredService<ServiceBusEventPublisher>());

        return services;
    }

    private static string BuildPostgreSqlConnectionString(IConfiguration configuration)
    {
        var host = configuration["Database:Host"] ?? "localhost";
        var port = configuration["Database:Port"] ?? "5432";
        var database = configuration["Database:Name"] ?? "smartcafe_menu";
        var username = configuration["Database:Username"] ?? "postgres";

        var password = configuration["Database:Password"]
            ?? configuration["ConnectionStrings:PostgreSQL:Password"]
            ?? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
            ?? throw new InvalidOperationException("Database password not configured");

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true";
    }
}
