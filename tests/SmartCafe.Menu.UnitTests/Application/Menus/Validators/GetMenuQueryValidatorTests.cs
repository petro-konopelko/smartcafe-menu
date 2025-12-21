using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.GetMenu.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class GetMenuQueryValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenIdsProvided()
    {
        // Arrange
        var validator = new GetMenuQueryValidator();
        var query = new GetMenuQuery(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsMissing()
    {
        // Arrange
        var validator = new GetMenuQueryValidator();
        var query = new GetMenuQuery(Guid.Empty, Guid.Empty);

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuIdRequired);
    }
}
