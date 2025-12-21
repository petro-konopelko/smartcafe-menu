using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class MenuItemDtoValidatorTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(200)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new MenuItemDtoValidator();

        var dto = new MenuItemDto(
            Id: null,
            Name: new string('a', length),
            Description: "Description",
            Price: new PriceDto(1, PriceUnit.PerItem, 0),
            Image: null,
            Ingredients: []);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    public void Validate_IsValid_WhenDescriptionAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new MenuItemDtoValidator();

        var dto = new MenuItemDto(
            Id: null,
            Name: "Item",
            Description: new string('a', length),
            Price: new PriceDto(1, PriceUnit.PerItem, 0),
            Image: null,
            Ingredients: []);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(201)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new MenuItemDtoValidator();

        var dto = new MenuItemDto(
            Id: null,
            Name: new string('a', length),
            Description: "Description",
            Price: new PriceDto(1, PriceUnit.PerItem, 0),
            Image: null,
            Ingredients: []);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ItemNameMaxLength);
    }

    [Theory]
    [InlineData(501)]
    [InlineData(1000)]
    public void Validate_ReturnsError_WhenDescriptionTooLong(int length)
    {
        // Arrange
        var validator = new MenuItemDtoValidator();

        var dto = new MenuItemDto(
            Id: null,
            Name: "Item",
            Description: new string('a', length),
            Price: new PriceDto(1, PriceUnit.PerItem, 0),
            Image: null,
            Ingredients: []);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.ItemDescriptionMaxLength);
    }
}
