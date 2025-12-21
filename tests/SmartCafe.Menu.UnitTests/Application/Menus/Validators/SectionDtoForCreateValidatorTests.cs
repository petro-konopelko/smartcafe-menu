using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class SectionDtoForCreateValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenIdsAreNull()
    {
        // Arrange
        var validator = new SectionDtoForCreateValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: null,
            AvailableTo: null,
            Items:
            [
                new MenuItemDto(
                    Id: null,
                    Name: "Item",
                    Description: null,
                    Price: new PriceDto(1, PriceUnit.PerItem, 0),
                    Image: null,
                    Ingredients: [])
            ]);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsError_WhenItemIdProvided()
    {
        // Arrange
        var validator = new SectionDtoForCreateValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: null,
            AvailableTo: null,
            Items:
            [
                new MenuItemDto(
                    Id: Guid.NewGuid(),
                    Name: "Item",
                    Description: null,
                    Price: new PriceDto(1, PriceUnit.PerItem, 0),
                    Image: null,
                    Ingredients: [])
            ]);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ItemIdMustBeNullForCreate);
    }
}
