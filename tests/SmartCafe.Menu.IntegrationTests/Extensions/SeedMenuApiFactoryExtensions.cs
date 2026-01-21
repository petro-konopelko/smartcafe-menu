using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using SmartCafe.Menu.IntegrationTests.DataSeeders;
using SmartCafe.Menu.IntegrationTests.Fixtures;
using SmartCafe.Menu.Shared.Providers;
using SmartCafe.Menu.Shared.Providers.Abstractions;
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

        using var scope = factory.Services.CreateScope();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        var cafe = CafeDataGenerator.GenerateValidCafe(dateTimeProvider, cafeId);
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();

        await DbSeeder.SeedCafeAsync(db, cafe, ct);
        await db.SaveChangesAsync(ct);
    }

    public static async Task<Domain.Entities.Cafe> SeedCafeAsync(
        this MenuApiTestFactory factory,
        string name,
        string? contactInfo,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        using var scope = factory.Services.CreateScope();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        var cafeId = Guid.CreateVersion7();
        var cafe = Domain.Entities.Cafe.Create(cafeId, name, dateTimeProvider, contactInfo).EnsureValue();
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();

        await DbSeeder.SeedCafeAsync(db, cafe, ct);
        await db.SaveChangesAsync(ct);

        return cafe;
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

        using var scope = factory.Services.CreateScope();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        var cafe = CafeDataGenerator.GenerateValidCafe(dateTimeProvider, cafeId);
        var menu = GenerateMenu(cafeId, state, name, sectionCount, itemsPerSection);
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

    public static async Task DeleteCafeAsync(
        this MenuApiTestFactory factory,
        Guid cafeId,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cafe = await db.Cafes.FindAsync([cafeId], ct);
        if (cafe is not null)
        {
            cafe.SoftDelete(dateTimeProvider);
            await db.SaveChangesAsync(ct);
        }
    }
}
