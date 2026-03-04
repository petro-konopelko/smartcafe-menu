using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class PriceDtoValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(99.99)]
    public void Validate_IsValid_WhenDiscountPercentInRange(decimal discountPercent)
    {
        // Arrange
        var validator = new PriceDtoValidator();
        var dto = new PriceDto(1, PriceUnit.PerItem, discountPercent);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenOutOfRange()
    {
        // Arrange
        var validator = new PriceDtoValidator();
        var dto = new PriceDto(0, (PriceUnit)999, 100);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ItemPriceGreaterThanZero);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PriceUnitInvalid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.DiscountPercentInvalid);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(100)]
    public void Validate_ReturnsCustomMessage_WhenDiscountPercentOutOfRange(decimal discountPercent)
    {
        // Arrange
        var validator = new PriceDtoValidator();
        var dto = new PriceDto(10, PriceUnit.PerItem, discountPercent);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.DiscountPercentInvalid);
    }
}
