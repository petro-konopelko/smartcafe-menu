using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Infrastructure.BlobStorage;
using SmartCafe.Menu.Infrastructure.Data;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Infrastructure.EventBus;
using SmartCafe.Menu.Infrastructure.Repositories;
using SmartCafe.Menu.Infrastructure.Services;

namespace SmartCafe.Menu.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Database
        var connectionString = configuration.GetConnectionString("MenuDb")
            ?? throw new InvalidOperationException("Database connection string 'MenuDb' not configured");
        services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(connectionString));

        // Repositories and UoW
        services.AddScoped<ICafeRepository, CafeRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Image processing and storage
        services.AddScoped<IImageProcessor, ImageProcessingService>();

        var blobStorageAccountName = configuration["AzureStorage:AccountName"]
            ?? throw new InvalidOperationException("Azure Storage account name not configured");
        var blobContainerName = configuration["AzureStorage:ContainerName"] ?? "menu-images";
        var blobServiceClient = new BlobServiceClient(
            new Uri($"https://{blobStorageAccountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        // AzureBlobStorageService must be Scoped because it depends on IImageProcessor which is Scoped
        services.AddScoped(sp =>
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

        // Correlation ID provider is registered in the API layer

        // Domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
