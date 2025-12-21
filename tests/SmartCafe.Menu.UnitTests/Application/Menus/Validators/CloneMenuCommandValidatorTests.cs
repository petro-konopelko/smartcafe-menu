using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class CloneMenuCommandValidatorTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(200)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new CloneMenuCommandValidator();
        var command = MenuTestData.CreateCloneMenuCommand(newName: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(201)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new CloneMenuCommandValidator();
        var command = MenuTestData.CreateCloneMenuCommand(newName: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsErrors_WhenNameMissing(string name)
    {
        // Arrange
        var validator = new CloneMenuCommandValidator();
        var command = new CloneMenuCommand(Guid.NewGuid(), Guid.NewGuid(), name);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameRequired);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsMissing()
    {
        // Arrange
        var validator = new CloneMenuCommandValidator();
        var command = new CloneMenuCommand(Guid.Empty, Guid.Empty, "Name");

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.SourceMenuIdRequired);
    }
}
