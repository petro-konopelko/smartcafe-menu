using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.UnitTests.Application.Menus.Validators;

public class SectionDtoValidatorTests
{
    [Fact]
    public void Validate_IsValid_WhenSectionValid()
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: TimeSpan.FromHours(9),
            AvailableTo: TimeSpan.FromHours(10),
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

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_IsValid_WhenNameAtLengthBoundaries(int length)
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: new string('a', length),
            AvailableFrom: null,
            AvailableTo: null,
            Items: CreateItems(count: 1));

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_IsValid_WhenItemCountAtBoundaries(int count)
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: null,
            AvailableTo: null,
            Items: CreateItems(count));

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(500)]
    public void Validate_ReturnsError_WhenNameTooLong(int length)
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: new string('a', length),
            AvailableFrom: null,
            AvailableTo: null,
            Items: CreateItems(count: 1));

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.SectionNameMaxLength);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    public void Validate_ReturnsError_WhenTooManyItems(int count)
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: null,
            AvailableTo: null,
            Items: CreateItems(count));

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.SectionMaxItems);
    }

    [Fact]
    public void Validate_ReturnsError_WhenAvailableFromNotLessThanTo()
    {
        // Arrange
        var validator = new SectionDtoValidator();

        var dto = new SectionDto(
            Id: null,
            Name: "Section",
            AvailableFrom: TimeSpan.FromHours(10),
            AvailableTo: TimeSpan.FromHours(9),
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
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.SectionAvailableFromLessThanTo);
    }

    private static IReadOnlyCollection<MenuItemDto> CreateItems(int count)
    {
        return [.. Enumerable.Range(0, count)
            .Select(i => new MenuItemDto(
                Id: null,
                Name: $"Item {i}",
                Description: null,
                Price: new PriceDto(1, PriceUnit.PerItem, 0),
                Image: null,
                Ingredients: []))];
    }
}
