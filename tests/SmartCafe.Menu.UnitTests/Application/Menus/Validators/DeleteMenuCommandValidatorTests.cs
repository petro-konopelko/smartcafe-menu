using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class DeleteMenuCommandValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenIdsProvided()
    {
        // Arrange
        var validator = new DeleteMenuCommandValidator();
        var command = new DeleteMenuCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsMissing()
    {
        // Arrange
        var validator = new DeleteMenuCommandValidator();
        var command = new DeleteMenuCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuIdRequired);
    }
}
