using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("MenuDb");
builder.Services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<MenuDbContext>();
    await context.Database.MigrateAsync();
}
