using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.IntegrationTests.DataSeeders;
using SmartCafe.Menu.IntegrationTests.Fixtures;
using SmartCafe.Menu.Shared.Providers;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.IntegrationTests.Extensions;

internal static class SeedMenuApiFactoryExtensions
{
    public static async Task SeedCafeAsync(
        this MenuApiTestFactory factory,
        Guid cafeId,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);
        var cafe = CafeDataGenerator.GenerateValidCafe(cafeId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();

        await DbSeeder.SeedCafeAsync(db, cafe, ct);
        await db.SaveChangesAsync(ct);
    }

    public static async Task<MenuEntity> SeedMenuAsync(
        this MenuApiTestFactory factory,
        Guid cafeId,
        MenuState state = MenuState.New,
        string? name = null,
        int sectionCount = 1,
        int itemsPerSection = 1,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var cafe = CafeDataGenerator.GenerateValidCafe(cafeId);
        var menu = GenerateMenu(cafeId, state, name, sectionCount, itemsPerSection);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();

        await DbSeeder.SeedCafeAsync(db, cafe, ct);
        await DbSeeder.SeedMenuAsync(db, menu, ct);
        await db.SaveChangesAsync(ct);

        return menu;
    }

    private static MenuEntity GenerateMenu(Guid cafeId, MenuState state, string? name, int sectionCount, int itemsPerSection)
    {
        var clock = new DateTimeProvider();
        var idProvider = new GuidIdProvider();

        var sections = Enumerable.Range(0, sectionCount)
            .Select(_ => MenuDataGenerator.GenerateSectionInfo(
                sectionId: null,
                itemCount: itemsPerSection))
            .ToArray();

        var menu = MenuEntity.Create(
            cafeId,
            name ?? MenuDataGenerator.GenerateValidMenuName(),
            idProvider,
            clock,
            sections).EnsureValue();

        if (state is MenuState.Published or MenuState.Active)
        {
            menu.Publish(clock).EnsureSuccess();
        }

        if (state is MenuState.Active)
        {
            menu.Activate(clock).EnsureSuccess();
        }

        return menu;
    }
}
