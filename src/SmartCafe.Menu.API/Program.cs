using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using FluentValidation;
using Serilog;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Infrastructure.Data;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.Infrastructure.DependencyInjection;
using SmartCafe.Menu.Infrastructure.Services;
using SmartCafe.Menu.Domain.Services;
using SmartCafe.Menu.API.Middleware;
using SmartCafe.Menu.API.Endpoints.Menus;
using SmartCafe.Menu.API.Endpoints.Images;
using Scalar.AspNetCore;

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

// Register handlers (Application layer) - only implemented handlers
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetMenu.GetMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.GetActiveMenuHandler>();
builder.Services.AddScoped<SmartCafe.Menu.Application.Features.Images.UploadImage.UploadImageHandler>();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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

menus.MapGetMenu();
menus.MapGetActiveMenu();

// Image upload endpoints
var images = api.MapGroup("/images");
images.MapUploadImage();

app.Run();

// Make Program accessible for testing
public partial class Program { }
