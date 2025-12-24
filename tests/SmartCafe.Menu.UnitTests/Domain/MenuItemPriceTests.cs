using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Domain;

public class MenuItemPriceTests
{
    private readonly Guid _cafeId = Guid.NewGuid();
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    [Theory]
    [InlineData(0, 0.1, ItemErrorCodes.PriceInvalid)]
    [InlineData(-5, 0.1, ItemErrorCodes.PriceInvalid)]
    [InlineData(10, -0.1, ItemErrorCodes.PriceDiscountInvalid)]
    [InlineData(10, 1, ItemErrorCodes.PriceDiscountInvalid)]
    [InlineData(10, 1.5, ItemErrorCodes.PriceDiscountInvalid)]
    public void Create_ReturnsValidationError_WhenPriceInvalid(decimal amount, decimal discount, string expectedCode)
    {
        // Arrange
        var sections = new[]
        {
            new SectionUpdateInfo(
                null,
                MenuDataGenerator.GenerateValidSectionName(),
                null,
                null,
                [new ItemUpdateInfo(
                    null,
                    MenuDataGenerator.GenerateValidItemName(),
                    MenuDataGenerator.GenerateValidProductDescription(),
                    new PriceUpdateInfo(amount, PriceUnit.PerItem, discount),
                    null,
                    [])])
        };

        // Act
        var result = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == expectedCode);
    }

    [Fact]
    public void SyncMenu_ReturnsValidationError_WhenPriceIsNull()
    {
        // Arrange
        var section = MenuDataGenerator.GenerateSectionInfo();

        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, [section]).EnsureValue();
        var item = section.Items.First();

        var updatedItem = new ItemUpdateInfo(
            item.Id,
            item.Name,
            item.Description,
            null!,
            null,
            []);

        var updatedSection = new SectionUpdateInfo(section.Id, section.Name, null, null, [updatedItem]);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == ItemErrorCodes.PriceAmountRequired);
    }
}
