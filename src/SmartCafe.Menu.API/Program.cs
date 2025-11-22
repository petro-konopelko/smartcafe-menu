using Azure.Identity;
using FluentValidation;
using Scalar.AspNetCore;
using Serilog;
using SmartCafe.Menu.API.Endpoints.Images;
using SmartCafe.Menu.API.Endpoints.Menus;
using SmartCafe.Menu.API.Middleware;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.Services;
using SmartCafe.Menu.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

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

// Domain and Infrastructure services
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddInfrastructure(builder.Configuration);

// Register handlers (Application layer)
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.CreateMenu.CreateMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.UpdateMenu.UpdateMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.CloneMenu.CloneMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetMenu.GetMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.GetActiveMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.ListMenus.ListMenusHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.PublishMenu.PublishMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.ActivateMenu.ActivateMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.DeleteMenu.DeleteMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Images.UploadImage.UploadImageHandler>();

// Register validators from Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<SmartCafe.Menu.Application.Features.Menus.CreateMenu.CreateMenuRequestValidator>();

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


// Map API endpoints
var api = app.MapGroup("/api");

// Menu endpoints
var cafes = api.MapGroup("/cafes/{cafeId:guid}");
var menus = cafes.MapGroup("/menus");

// CRUD operations
menus.MapCreateMenu();          // POST /api/cafes/{cafeId}/menus
menus.MapUpdateMenu();          // PUT /api/cafes/{cafeId}/menus/{menuId}
menus.MapGetMenu();             // GET /api/cafes/{cafeId}/menus/{menuId}
menus.MapListMenus();           // GET /api/cafes/{cafeId}/menus
menus.MapDeleteMenu();          // DELETE /api/cafes/{cafeId}/menus/{menuId}

// Menu operations
menus.MapCloneMenu();           // POST /api/cafes/{cafeId}/menus/{menuId}/clone
menus.MapPublishMenu();         // POST /api/cafes/{cafeId}/menus/{menuId}/publish
menus.MapActivateMenu();        // POST /api/cafes/{cafeId}/menus/{menuId}/activate
menus.MapGetActiveMenu();       // GET /api/cafes/{cafeId}/menus/active

// Image upload endpoints
var images = api.MapGroup("/images");
images.MapUploadImage();

app.Run();

// Make Program accessible for testing
public partial class Program { }
