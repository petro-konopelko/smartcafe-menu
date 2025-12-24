using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Domain;

public class MenuItemTests
{
    private readonly Guid _cafeId = Guid.NewGuid();
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SyncMenu_ReturnsValidationError_WhenItemNameMissing(string? invalidName)
    {
        // Arrange
        var section = MenuDataGenerator.GenerateSectionInfo();
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, [section]).EnsureValue();
        var item = section.Items.First();

        var updatedItem = new ItemUpdateInfo(
            item.Id,
            invalidName!,
            item.Description,
            new PriceUpdateInfo(item.Price.Amount, item.Price.Unit, item.Price.Discount),
            null,
            []);

        var updatedSection = new SectionUpdateInfo(section.Id, section.Name, null, null, [updatedItem]);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == ItemErrorCodes.ItemNameRequired);
    }

    [Theory]
    [InlineData("Burger", "burger ")]
    [InlineData("Pizza", " PIZZA  ")]
    [InlineData("Salad", "salad")]
    public void Create_ReturnsValidationError_WhenItemNamesNotUniqueWithinSection(string itemName1, string itemName2)
    {
        // Arrange
        var sections = new[]
        {
            new SectionUpdateInfo(
                null,
               MenuDataGenerator.GenerateValidSectionName(),
                null,
                null,
                [
                    new ItemUpdateInfo(null, itemName1, "Desc1", new PriceUpdateInfo(10m, PriceUnit.PerItem, 0m), null, []),
                    new ItemUpdateInfo(null, itemName2, "Desc2", new PriceUpdateInfo(12m, PriceUnit.PerItem, 0m), null, [])
                ])
        };

        // Act
        var result = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);
        var error = Assert.Single(result.Error!.Details);
        Assert.Equal(ItemErrorCodes.ItemNameNotUnique, error.Code);
    }

    [Fact]
    public void SyncMenu_ReturnsValidationError_WhenDuplicateItemIdsProvided()
    {
        // Arrange
        SectionUpdateInfo[] initialSections = [MenuDataGenerator.GenerateSectionInfo(itemCount: 2)];

        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, initialSections).EnsureValue();
        var section = menu.Sections.First();
        var firstItem = section.Items.First();
        var secondItem = section.Items.Last();

        var duplicateIdItems = new[]
        {
            new ItemUpdateInfo(firstItem.Id, firstItem.Name, firstItem.Description, new PriceUpdateInfo(firstItem.Price.Amount, firstItem.Price.Unit, firstItem.Price.Discount), null, []),
            new ItemUpdateInfo(firstItem.Id, secondItem.Name, secondItem.Description, new PriceUpdateInfo(secondItem.Price.Amount, secondItem.Price.Unit, secondItem.Price.Discount), null, [])
        };

        var updatedSection = new SectionUpdateInfo(section.Id, section.Name, null, null, duplicateIdItems);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);
        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(ItemErrorCodes.DuplicateItemId, errorDetail.Code);
    }

    [Fact]
    public void SyncMenu_ReturnsValidationError_WhenItemIdNotFound()
    {
        // Arrange
        var section = MenuDataGenerator.GenerateSectionInfo();
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, [section]).EnsureValue();
        var item = section.Items.First();

        var nonExistentItemId = Guid.NewGuid();

        var updatedItem = new ItemUpdateInfo(
            nonExistentItemId,
            item.Name,
            item.Description,
            new PriceUpdateInfo(item.Price.Amount, item.Price.Unit, item.Price.Discount),
            null,
            []);

        var updatedSection = new SectionUpdateInfo(section.Id, section.Name, null, null, [updatedItem]);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(ItemErrorCodes.ItemNotFound, errorDetail.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SyncMenu_ReturnsValidationError_WhenIngredientNameMissing(string? invalidIngredientName)
    {
        // Arrange
        SectionUpdateInfo[] initialSections = [MenuDataGenerator.GenerateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, initialSections).EnsureValue();
        var section = menu.Sections.First();
        var item = section.Items.First();

        var updatedItem = new ItemUpdateInfo(
            item.Id,
            item.Name,
            item.Description,
                new PriceUpdateInfo(item.Price.Amount, item.Price.Unit, item.Price.Discount),
                null,
                [new IngredientItemUpdate(invalidIngredientName!, true)]);

        var updatedSection = new SectionUpdateInfo(section.Id, section.Name, null, null, [updatedItem]);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == ItemErrorCodes.IngredientNameRequired);
    }

    [Theory]
    [InlineData(null, "/images/thumbnail.jpg")]
    [InlineData("/images/original.jpg", null)]
    public void SyncMenu_ReturnsValidationError_WhenImagePathsInvalid(string? originalPath, string? thumbnailPath)
    {
        // Arrange
        var initialSection = MenuDataGenerator.GenerateSectionInfo();
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, [initialSection]).EnsureValue();
        var item = initialSection.Items.First();

        var invalidImageUpdate = new ImageUpdateInfo(originalPath, thumbnailPath);

        var updatedItem = new ItemUpdateInfo(
                item.Id,
                item.Name,
                item.Description,
                new PriceUpdateInfo(item.Price.Amount, item.Price.Unit, item.Price.Discount),
                invalidImageUpdate,
                []);

        var updatedSection = new SectionUpdateInfo(initialSection.Id, initialSection.Name, null, null, [updatedItem]);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == ItemErrorCodes.ImageAssetPathsInvalid);
    }
}
