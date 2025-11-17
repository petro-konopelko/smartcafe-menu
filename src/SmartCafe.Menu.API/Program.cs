using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using Azure.Identity;
using FluentValidation;
using Serilog;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Infrastructure.Data;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Infrastructure.Repositories;
using SmartCafe.Menu.Infrastructure.Services;
using SmartCafe.Menu.Infrastructure.BlobStorage;
using SmartCafe.Menu.Infrastructure.EventBus;
using SmartCafe.Menu.API.Middleware;
using SmartCafe.Menu.API.Endpoints.Menus;
using SmartCafe.Menu.API.Endpoints.Categories;
using SmartCafe.Menu.API.Endpoints.Images;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure secrets (Key Vault in production, User Secrets in dev)
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVault:Uri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

// Build PostgreSQL connection string dynamically
var connectionString = BuildPostgreSqlConnectionString(builder.Configuration);

// Add services to the container
builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IImageProcessor, ImageProcessingService>();

// Register handlers
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.UpsertMenu.UpsertMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.ActivateMenu.ActivateMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.PublishMenu.PublishMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.DeleteMenu.DeleteMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetMenu.GetMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.ListMenus.ListMenusHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.GetActiveMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Categories.CreateCategory.CreateCategoryHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Categories.UpdateCategory.UpdateCategoryHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Categories.DeleteCategory.DeleteCategoryHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Categories.ListCategories.ListCategoriesHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Images.UploadImage.UploadImageHandler>();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure Azure Blob Storage
var blobStorageAccountName = builder.Configuration["AzureStorage:AccountName"] 
    ?? throw new InvalidOperationException("Azure Storage account name not configured");
var blobContainerName = builder.Configuration["AzureStorage:ContainerName"] ?? "menu-images";
var blobServiceClient = new BlobServiceClient(
    new Uri($"https://{blobStorageAccountName}.blob.core.windows.net"),
    new DefaultAzureCredential());
builder.Services.AddSingleton(sp => 
{
    var imageProcessor = sp.GetRequiredService<IImageProcessor>();
    return new AzureBlobStorageService(blobServiceClient, blobContainerName, imageProcessor);
});
builder.Services.AddScoped<IImageStorageService>(sp => sp.GetRequiredService<AzureBlobStorageService>());

// Configure Azure Service Bus
var serviceBusNamespace = builder.Configuration["AzureServiceBus:Namespace"]
    ?? throw new InvalidOperationException("Service Bus namespace not configured");
var serviceBusClient = new ServiceBusClient(
    $"{serviceBusNamespace}.servicebus.windows.net",
    new DefaultAzureCredential());
var topicName = builder.Configuration["AzureServiceBus:TopicName"] ?? "menu-events";
builder.Services.AddSingleton(sp => new ServiceBusEventPublisher(serviceBusClient, topicName));
builder.Services.AddScoped<IEventPublisher>(sp => sp.GetRequiredService<ServiceBusEventPublisher>());

// Add API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Map API endpoints
var api = app.MapGroup("/api");

// Menu endpoints
var cafes = api.MapGroup("/cafes/{cafeId:guid}");
var menus = cafes.MapGroup("/menus");

menus.MapUpsertMenu(); // Main endpoint for menu configuration UI
menus.MapGetMenu();
menus.MapListMenus();
menus.MapDeleteMenu();
menus.MapPublishMenu();
menus.MapActivateMenu();
menus.MapGetActiveMenu();

// Image upload endpoints
var images = api.MapGroup("/images");
images.MapUploadImage();

// Category endpoints
var categories = api.MapGroup("/categories");
categories.MapListCategories();
categories.MapCreateCategory();
categories.MapUpdateCategory();
categories.MapDeleteCategory();

app.Run();

// Helper method to build PostgreSQL connection string dynamically
static string BuildPostgreSqlConnectionString(IConfiguration configuration)
{
    var host = configuration["Database:Host"] ?? "localhost";
    var port = configuration["Database:Port"] ?? "5432";
    var database = configuration["Database:Name"] ?? "smartcafe_menu";
    var username = configuration["Database:Username"] ?? "postgres";
    
    // Password from environment variable or Key Vault (never from appsettings.json)
    var password = configuration["Database:Password"] 
        ?? configuration["ConnectionStrings:PostgreSQL:Password"]
        ?? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
        ?? throw new InvalidOperationException("Database password not configured");
    
    return $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true";
}

// Make Program accessible for testing
public partial class Program { }
