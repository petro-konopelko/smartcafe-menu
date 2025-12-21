using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class PublishMenuCommandValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenIdsProvided()
    {
        // Arrange
        var validator = new PublishMenuCommandValidator();
        var command = new PublishMenuCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsMissing()
    {
        // Arrange
        var validator = new PublishMenuCommandValidator();
        var command = new PublishMenuCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuIdRequired);
    }
}
