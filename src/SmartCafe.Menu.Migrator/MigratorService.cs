using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

namespace SmartCafe.Menu.Migrator;

public class MigratorService(
    ILogger<MigratorService> logger,
    MenuDbContext context)
{
    private readonly ILogger<MigratorService> _logger = logger;
    private readonly MenuDbContext _context = context;

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting database migration...");
        await _context.Database.MigrateAsync();
        _logger.LogInformation("Database migration completed successfully");
    }
}
