using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class IngredientValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenNamePresent()
    {
        // Arrange
        var validator = new IngredientValidator();
        var dto = new IngredientDto("Milk", true);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new IngredientValidator();
        var dto = new IngredientDto(new string('a', length), true);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenNameMissing()
    {
        // Arrange
        var validator = new IngredientValidator();
        var dto = new IngredientDto("", true);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.IngredientNameRequired);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new IngredientValidator();
        var dto = new IngredientDto(new string('a', length), true);

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.IngredientNameMaxLength);
    }
}
