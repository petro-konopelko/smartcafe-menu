using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class UpdateMenuCommandValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenCommandValid()
    {
        // Arrange
        var validator = new UpdateMenuCommandValidator();
        var command = MenuTestData.CreateValidUpdateMenuCommand();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(200)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new UpdateMenuCommandValidator();
        var command = MenuTestData.CreateValidUpdateMenuCommand(name: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsMissing()
    {
        // Arrange
        var validator = new UpdateMenuCommandValidator();

        var command = new UpdateMenuCommand(
            CafeId: Guid.Empty,
            MenuId: Guid.Empty,
            Name: "",
            Sections: []);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuMustHaveSection);
    }

    [Theory]
    [InlineData(201)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new UpdateMenuCommandValidator();
        var command = MenuTestData.CreateValidUpdateMenuCommand(name: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameMaxLength);
    }
}
