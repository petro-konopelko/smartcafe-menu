using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class CreateMenuCommandValidatorTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(200)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new CreateMenuCommandValidator();
        var command = MenuTestData.CreateValidCreateMenuCommand(name: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReturnsErrors_WhenIdsOrNameMissing()
    {
        // Arrange
        var validator = new CreateMenuCommandValidator();

        var command = new CreateMenuCommand(
            CafeId: Guid.Empty,
            Name: "",
            Sections: []);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.CafeIdRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameRequired);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuMustHaveSection);
    }

    [Fact]
    public void Validate_ReturnsError_WhenSectionIdProvided()
    {
        // Arrange
        var validator = new CreateMenuCommandValidator();

        var command = new CreateMenuCommand(
            CafeId: Guid.NewGuid(),
            Name: "Menu",
            Sections:
            [
                new SectionDto(
                    Id: Guid.NewGuid(),
                    Name: "Breakfast",
                    AvailableFrom: null,
                    AvailableTo: null,
                    Items:
                    [
                        new MenuItemDto(
                            Id: null,
                            Name: "Coffee",
                            Description: null,
                            Price: new PriceDto(1, Menu.Domain.Enums.PriceUnit.PerItem, 0),
                            Image: null,
                            Ingredients: [])
                    ])
            ]);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.SectionIdMustBeNullForCreate);
    }

    [Theory]
    [InlineData(201)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new CreateMenuCommandValidator();

        var command = MenuTestData.CreateValidCreateMenuCommand(name: new string('a', length));

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.MenuNameMaxLength);
    }
}
