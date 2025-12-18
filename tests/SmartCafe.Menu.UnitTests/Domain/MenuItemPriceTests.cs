using Bogus;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.UnitTests.DataGenerators;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Domain;

public class MenuItemPriceTests
{
    private readonly Guid _cafeId = Guid.NewGuid();
    private readonly Faker _faker = new();
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
                _faker.Commerce.Department(),
                null,
                null,
                [new ItemUpdateInfo(
                    null,
                    _faker.Commerce.ProductName(),
                    _faker.Commerce.ProductDescription(),
                    new PriceUpdateInfo(amount, PriceUnit.PerItem, discount),
                    null,
                    [])])
        };

        // Act
        var result = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections);

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
        var section = UpdateInfoDataGenerator.GenerateUpdateSectionInfo();

        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, [section]).EnsureValue();
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
        var result = menu.SyncMenu(_faker.Company.CatchPhrase(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Details, d => d.Code == ItemErrorCodes.PriceAmountRequired);
    }
}
