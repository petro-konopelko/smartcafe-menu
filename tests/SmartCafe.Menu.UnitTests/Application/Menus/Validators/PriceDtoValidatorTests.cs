using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class PriceDtoValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenPriceValid()
    {
        // Arrange
        var validator = new PriceDtoValidator();
        var dto = new PriceDto(1, PriceUnit.PerItem, 0);

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
        var dto = new PriceDto(0, (PriceUnit)999, 2);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ItemPriceGreaterThanZero);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.PriceUnitInvalid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.DiscountInvalid);
    }
}
