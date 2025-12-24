using Bogus;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public static class MenuTestData
{
    public static Faker CreateFaker() => new();

    public static CreateMenuCommand CreateValidCreateMenuCommand(
        Guid? cafeId = null,
        string? name = null,
        int sectionCount = 1,
        int itemsPerSection = 1,
        bool includeImages = false)
    {
        var faker = CreateFaker();

        var sections = Enumerable.Range(0, sectionCount)
            .Select(_ => CreateValidSectionForCreate(faker, itemsPerSection, includeImages))
            .ToList();

        return new CreateMenuCommand(
            cafeId ?? Guid.NewGuid(),
            name ?? faker.Company.CatchPhrase(),
            sections);
    }

    public static UpdateMenuCommand CreateValidUpdateMenuCommand(
        Guid? cafeId = null,
        Guid? menuId = null,
        string? name = null,
        int sectionCount = 1,
        int itemsPerSection = 1,
        bool includeImages = false)
    {
        var faker = CreateFaker();

        var sections = Enumerable.Range(0, sectionCount)
            .Select(_ => CreateValidSectionForUpdate(faker, itemsPerSection, includeImages))
            .ToList();

        return new UpdateMenuCommand(
            cafeId ?? Guid.NewGuid(),
            menuId ?? Guid.NewGuid(),
            name ?? faker.Company.CatchPhrase(),
            sections);
    }

    public static PublishMenuCommand CreatePublishMenuCommand(Guid? cafeId = null, Guid? menuId = null)
        => new(cafeId ?? Guid.NewGuid(), menuId ?? Guid.NewGuid());

    public static ActivateMenuCommand CreateActivateMenuCommand(Guid? cafeId = null, Guid? menuId = null)
        => new(cafeId ?? Guid.NewGuid(), menuId ?? Guid.NewGuid());

    public static DeleteMenuCommand CreateDeleteMenuCommand(Guid? cafeId = null, Guid? menuId = null)
        => new(cafeId ?? Guid.NewGuid(), menuId ?? Guid.NewGuid());

    public static CloneMenuCommand CreateCloneMenuCommand(Guid? cafeId = null, Guid? sourceMenuId = null, string? newName = null)
    {
        var faker = CreateFaker();
        return new CloneMenuCommand(
            cafeId ?? Guid.NewGuid(),
            sourceMenuId ?? Guid.NewGuid(),
            newName ?? faker.Company.CatchPhrase());
    }

    public static GetMenuQuery CreateGetMenuQuery(Guid? cafeId = null, Guid? menuId = null)
        => new(cafeId ?? Guid.NewGuid(), menuId ?? Guid.NewGuid());

    public static GetActiveMenuQuery CreateGetActiveMenuQuery(Guid? cafeId = null)
        => new(cafeId ?? Guid.NewGuid());

    public static MenuEntity CreateNewMenu(
        Guid cafeId,
        IGuidIdProvider idProvider,
        FakeDateTimeProvider clock,
        string? name = null,
        SectionUpdateInfo[]? sections = null)
    {
        var faker = CreateFaker();
        var menuName = name ?? faker.Company.CatchPhrase();

        sections ??= [MenuDataGenerator.GenerateSectionInfo(itemCount: 1)];

        return MenuEntity.Create(cafeId, menuName, idProvider, clock, sections).EnsureValue();
    }

    public static MenuEntity CreatePublishedMenu(Guid cafeId, IGuidIdProvider idProvider, FakeDateTimeProvider clock)
    {
        var menu = CreateNewMenu(cafeId, idProvider, clock);
        var result = menu.Publish(clock);
        EnsureResult(result);
        return menu;
    }

    public static MenuEntity CreateActiveMenu(Guid cafeId, IGuidIdProvider idProvider, FakeDateTimeProvider clock)
    {
        var menu = CreatePublishedMenu(cafeId, idProvider, clock);
        var result = menu.Activate(clock);
        EnsureResult(result);
        return menu;
    }

    public static UpdateMenuCommand CreateUpdateMenuCommandFromMenu(
        MenuEntity menu,
        Guid cafeId,
        string? newName = null)
    {
        ArgumentNullException.ThrowIfNull(menu);

        var faker = CreateFaker();

        var sections = menu.Sections
            .OrderBy(s => s.Position)
            .Select(s => new SectionDto(
                Id: s.Id,
                Name: s.Name,
                AvailableFrom: s.AvailableFrom,
                AvailableTo: s.AvailableTo,
                Items: [.. s.Items
                    .OrderBy(i => i.Position)
                    .Select(i => new MenuItemDto(
                        Id: i.Id,
                        Name: i.Name,
                        Description: i.Description,
                        Price: new PriceDto(i.Price.Amount, i.Price.Unit, i.Price.Discount),
                        Image: i.Image is null || (string.IsNullOrWhiteSpace(i.Image.OriginalPath) && string.IsNullOrWhiteSpace(i.Image.ThumbnailPath))
                            ? null
                            : new MenuItemImageDto(i.Image.OriginalPath!, i.Image.ThumbnailPath!),
                        Ingredients: [.. i.IngredientOptions.Select(ing => new IngredientDto(ing.Name, ing.IsExcludable))]
                    ))]
            ))
            .ToList();

        return new UpdateMenuCommand(
            cafeId,
            menu.Id,
            newName ?? faker.Company.CatchPhrase(),
            sections);
    }

    private static void EnsureResult(Result result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException(string.Join(", ", result.EnsureError().Details.Select(d => d.Code)));
        }
    }

    private static SectionDto CreateValidSectionForCreate(Faker faker, int itemsPerSection, bool includeImages)
    {
        return new SectionDto(
            Id: null,
            Name: faker.Commerce.Department(),
            AvailableFrom: TimeSpan.FromHours(8),
            AvailableTo: TimeSpan.FromHours(18),
            Items: [.. Enumerable.Range(0, itemsPerSection).Select(_ => CreateValidItemForCreate(faker, includeImages))]);
    }

    private static SectionDto CreateValidSectionForUpdate(Faker faker, int itemsPerSection, bool includeImages)
    {
        return new SectionDto(
            Id: Guid.NewGuid(),
            Name: faker.Commerce.Department(),
            AvailableFrom: TimeSpan.FromHours(8),
            AvailableTo: TimeSpan.FromHours(18),
            Items: [.. Enumerable.Range(0, itemsPerSection).Select(_ => CreateValidItemForUpdate(faker, includeImages))]);
    }

    private static MenuItemDto CreateValidItemForCreate(Faker faker, bool includeImages)
    {
        return new MenuItemDto(
            Id: null,
            Name: faker.Commerce.ProductName(),
            Description: faker.Commerce.ProductDescription(),
            Price: new PriceDto(faker.Random.Decimal(1, 100), faker.PickRandom<PriceUnit>(), faker.Random.Decimal(0, 0.99m)),
            Image: includeImages ? CreateValidImage(faker) : null,
            Ingredients: [new IngredientDto(faker.Commerce.ProductMaterial(), faker.Random.Bool())]);
    }

    private static MenuItemDto CreateValidItemForUpdate(Faker faker, bool includeImages)
    {
        return new MenuItemDto(
            Id: Guid.NewGuid(),
            Name: faker.Commerce.ProductName(),
            Description: faker.Commerce.ProductDescription(),
            Price: new PriceDto(faker.Random.Decimal(1, 100), faker.PickRandom<PriceUnit>(), faker.Random.Decimal(0, 0.99m)),
            Image: includeImages ? CreateValidImage(faker) : null,
            Ingredients: [new IngredientDto(faker.Commerce.ProductMaterial(), faker.Random.Bool())]);
    }

    private static MenuItemImageDto CreateValidImage(Faker faker)
    {
        var original = $"images/{faker.Random.Guid()}/original.jpg";
        var thumbnail = $"images/{faker.Random.Guid()}/thumbnail.jpg";
        return new MenuItemImageDto(original, thumbnail);
    }
}
