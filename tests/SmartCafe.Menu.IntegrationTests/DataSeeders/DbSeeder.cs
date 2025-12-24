using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.IntegrationTests.DataSeeders;

public static class DbSeeder
{
    public static async Task SeedCafeAsync(MenuDbContext dbContext, Cafe cafe, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(cafe);

        if (await dbContext.Cafes.FindAsync([cafe.Id], ct) is not null)
        {
            return;
        }

        await dbContext.Cafes.AddAsync(cafe, ct);
    }

    public static async Task SeedMenuAsync(MenuDbContext dbContext, MenuEntity menu, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(menu);

        await dbContext.Menus.AddAsync(menu, ct);
    }
}
