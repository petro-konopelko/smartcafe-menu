using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Domain;

public class MenuSectionTests
{
    private readonly Guid _cafeId = Guid.NewGuid();
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    [Fact]
    public void Create_ReturnsValidationError_WhenSectionHasMoreThan100Items()
    {
        // Arrange
        SectionUpdateInfo[] sections = [
            MenuDataGenerator.GenerateSectionInfo(itemCount: 101)
        ];

        // Act
        var result = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.TooManyItems, errorDetail.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ReturnsValidationError_WhenSectionNameMissing(string? invalidName)
    {
        // Arrange
        var section = MenuDataGenerator.GenerateSectionInfo();

        // Act
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, [section]).EnsureValue();

        var updatedSection = new SectionUpdateInfo(section.Id, invalidName!, null, null, section.Items);
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.SectionNameRequired, errorDetail.Code);
    }

    [Theory]
    [InlineData("Coffee", "coffee ")]
    [InlineData("Breakfast", " BREAKFAST  ")]
    [InlineData("Lunch", "lunch")]
    public void Create_ReturnsValidationError_WhenSectionNamesNotUnique(string sectionName1, string sectionName2)
    {
        // Arrange
        var sections = new[]
        {
            new SectionUpdateInfo(null, sectionName1, null, null, [new ItemUpdateInfo(null, "Item1", "Desc", new PriceUpdateInfo(10m, PriceUnit.PerItem, 0m), null, [])]),
            new SectionUpdateInfo(null, sectionName2, null, null, [new ItemUpdateInfo(null, "Item2", "Desc", new PriceUpdateInfo(10m, PriceUnit.PerItem, 0m), null, [])])
        };

        // Act
        var result = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.SectionNameNotUnique, errorDetail.Code);
    }

    [Fact]
    public void Create_ReturnsValidationError_WhenSectionIdsNotUnique()
    {
        // Arrange
        var duplicateId = Guid.NewGuid();
        var sections = new[]
        {
            new SectionUpdateInfo(duplicateId, "Section1", null, null, []),
            new SectionUpdateInfo(duplicateId, "Section2", null, null, [])
        };

        // Act
        var result = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.DuplicateSectionId, errorDetail.Code);
    }

    [Fact]
    public void SyncMenu_ReturnsValidationError_WhenAvailabilityWindowInvalid()
    {
        // Arrange
        var invalidFrom = TimeSpan.FromHours(15);
        var invalidTo = TimeSpan.FromHours(9);

        var initialSections = new[] { MenuDataGenerator.GenerateSectionInfo() };
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, initialSections).EnsureValue();
        var section = menu.Sections.First();

        var updatedSections = new[]
        {
            new SectionUpdateInfo(section.Id, section.Name, invalidFrom, invalidTo, initialSections.First().Items)
        };

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), updatedSections, _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.InvalidAvailabilityWindow, errorDetail.Code);
    }

    [Fact]
    public void SyncMenu_ReturnsSectionNotFoundError_WhenSectionIdDoesNotExist()
    {
        // Arrange
        var section = MenuDataGenerator.GenerateSectionInfo();
        SectionUpdateInfo[] initialSections = [section];
        var menu = MenuEntity.Create(_cafeId, MenuDataGenerator.GenerateValidMenuName(), _idProvider, _clock, initialSections).EnsureValue();


        var nonExistentSectionId = Guid.NewGuid();
        var updatedSection = new SectionUpdateInfo(nonExistentSectionId, section.Name, null, null, section.Items);

        // Act
        var result = menu.SyncMenu(MenuDataGenerator.GenerateValidMenuName(), [updatedSection], _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);

        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(SectionErrorCodes.SectionNotFound, errorDetail.Code);
    }
}
