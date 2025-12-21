using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Validators;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class ListMenusQueryValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenCafeIdProvided()
    {
        // Arrange
        var validator = new ListMenusQueryValidator();
        var query = new ListMenusQuery(Guid.NewGuid());

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsError_WhenCafeIdMissing()
    {
        // Arrange
        var validator = new ListMenusQueryValidator();
        var query = new ListMenusQuery(Guid.Empty);

        // Act
        var result = validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
    }
}
