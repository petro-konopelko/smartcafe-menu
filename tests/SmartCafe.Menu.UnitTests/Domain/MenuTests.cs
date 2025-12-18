using Bogus;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;
using SmartCafe.Menu.UnitTests.DataGenerators;
using SmartCafe.Menu.UnitTests.Extensions;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Domain;

public class MenuTests
{
    private readonly Guid _cafeId = Guid.NewGuid();
    private readonly Faker _faker = new();
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    // ==================== HAPPY PATH TESTS ====================

    [Fact]
    public void Create_ReturnsCompleteMenuWithCorrectStructure_WhenAllInputValid()
    {
        // Arrange
        var menuName = _faker.Company.CatchPhrase();
        var createdAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(createdAt);

        SectionUpdateInfo[] sections = [
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo(itemCount: 2),
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo()
        ];

        // Act
        var result = MenuEntity.Create(_cafeId, menuName, _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsSuccess);
        var menu = result.EnsureValue();

        menu.VerifyMenu(
            _cafeId,
            menuName,
            createdAt,
            createdAt,
            sections);

        var createdEvent = Assert.IsType<MenuCreatedEvent>(Assert.Single(menu.DomainEvents));
        Assert.Multiple(
            () => Assert.Equal(menu.Id, createdEvent.MenuId),
            () => Assert.Equal(_cafeId, createdEvent.CafeId),
            () => Assert.Equal(menuName, createdEvent.MenuName),
            () => Assert.Equal(createdAt, createdEvent.Timestamp));
    }

    [Fact]
    public void SyncMenu_UpdatesMenuStructureCorrectly_WhenValidChanges()
    {
        // Arrange
        var initialMenuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] initialSections = [
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo(itemCount: 3),
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo()
        ];

        var createdAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(createdAt);

        var menu = MenuEntity.Create(_cafeId, initialMenuName, _idProvider, _clock, initialSections).EnsureValue();

        var updatedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(updatedAt);

        var firstSection = menu.Sections.First();

        SectionUpdateInfo[] updatedSections = [
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo(
                firstSection.Id,
                itemCount: 2,
                [null, firstSection.Items.First().Id]),
            UpdateInfoDataGenerator.GenerateUpdateSectionInfo()
        ];

        var updatedMenuName = initialMenuName + " - Updated";

        // Act
        var result = menu.SyncMenu(updatedMenuName, updatedSections, _clock, _idProvider);

        // Assert
        Assert.True(result.IsSuccess);
        menu.VerifyMenu(
            _cafeId,
            updatedMenuName,
            createdAt,
            updatedAt,
            updatedSections);
    }

    [Fact]
    public void Publish_SetsPublishedStateAndAddsEvent_WhenMenuValid()
    {
        // Arrange
        var menuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, menuName, _idProvider, _clock, sections).EnsureValue();
        menu.ClearDomainEvents();

        var publishedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(publishedAt);

        // Act
        var result = menu.Publish(_clock);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Multiple(
            () => Assert.Equal(MenuState.Published, menu.State),
            () => Assert.Equal(publishedAt, menu.PublishedAt),
            () => Assert.Equal(publishedAt, menu.UpdatedAt));

        var publishedEvent = Assert.IsType<MenuPublishedEvent>(Assert.Single(menu.DomainEvents));
        Assert.Multiple(
            () => Assert.Equal(menu.Id, publishedEvent.MenuId),
            () => Assert.Equal(_cafeId, publishedEvent.CafeId),
            () => Assert.Equal(menuName, publishedEvent.MenuName),
            () => Assert.Equal(publishedAt, publishedEvent.Timestamp));
    }

    [Fact]
    public void Activate_SetsActiveStateAndAddsEvent_WhenMenuPublished()
    {
        // Arrange
        var menuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, menuName, _idProvider, _clock, sections).EnsureValue();
        menu.Publish(_clock);
        menu.ClearDomainEvents();

        var activatedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(activatedAt);

        // Act
        var result = menu.Activate(_clock);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Multiple(
            () => Assert.Equal(MenuState.Active, menu.State),
            () => Assert.Equal(activatedAt, menu.ActivatedAt),
            () => Assert.Equal(activatedAt, menu.UpdatedAt));

        var activatedEvent = Assert.IsType<MenuActivatedEvent>(Assert.Single(menu.DomainEvents));
        Assert.Multiple(
            () => Assert.Equal(menu.Id, activatedEvent.MenuId),
            () => Assert.Equal(_cafeId, activatedEvent.CafeId),
            () => Assert.Equal(menuName, activatedEvent.MenuName),
            () => Assert.Equal(activatedAt, activatedEvent.Timestamp));
    }

    [Fact]
    public void Deactivate_SetsPublishedStateAndAddsEvent_WhenMenuActive()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        menu.Publish(_clock);
        menu.Activate(_clock);
        menu.ClearDomainEvents();

        var deactivatedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(deactivatedAt);

        // Act
        var result = menu.Deactivate(_clock);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Multiple(
            () => Assert.Equal(MenuState.Published, menu.State),
            () => Assert.Equal(deactivatedAt, menu.UpdatedAt));

        var deactivatedEvent = Assert.IsType<MenuDeactivatedEvent>(Assert.Single(menu.DomainEvents));
        Assert.Multiple(
            () => Assert.Equal(menu.Id, deactivatedEvent.MenuId),
            () => Assert.Equal(_cafeId, deactivatedEvent.CafeId),
            () => Assert.Equal(deactivatedAt, deactivatedEvent.Timestamp));
    }

    [Theory]
    [InlineData(MenuState.New)]
    [InlineData(MenuState.Published)]
    public void SoftDelete_SetsDeletedStateAndAddsEvent_WhenMenuIsInState(MenuState targetState)
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        ChangeState(menu, targetState);
        menu.ClearDomainEvents();

        var deletedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(deletedAt);

        // Act
        var result = menu.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Multiple(
            () => Assert.Equal(MenuState.Deleted, menu.State),
            () => Assert.Equal(deletedAt, menu.UpdatedAt));

        var deletedEvent = Assert.IsType<MenuDeletedEvent>(Assert.Single(menu.DomainEvents));
        Assert.Multiple(
            () => Assert.Equal(menu.Id, deletedEvent.MenuId),
            () => Assert.Equal(_cafeId, deletedEvent.CafeId),
            () => Assert.Equal(deletedAt, deletedEvent.Timestamp));
    }

    [Fact]
    public void SoftDelete_ReturnsSuccess_WhenMenuAlreadyDeleted()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var createdAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(createdAt);
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        menu.SoftDelete(_clock);
        menu.ClearDomainEvents();

        var deletedAt = _faker.Date.Recent().ToUniversalTime();
        _clock.SetUtcNow(deletedAt);

        // Act
        var result = menu.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Multiple(
            () => Assert.Equal(MenuState.Deleted, menu.State),
            () => Assert.Equal(createdAt, menu.UpdatedAt),
            () => Assert.Empty(menu.DomainEvents));
    }

    // ==================== NEGATIVE TESTS ====================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ReturnsValidationError_WhenNameMissing(string? menuName)
    {
        // Arrange
        var sections = Array.Empty<SectionUpdateInfo>();

        // Act
        var result = MenuEntity.Create(_cafeId, menuName!, _idProvider, _clock, sections);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);

        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuNameRequired, errorDetail.Code);
    }

    [Fact]
    public void Create_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        var menuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            MenuEntity.Create(_cafeId, menuName, _idProvider, null!, sections));
    }

    [Fact]
    public void Create_ThrowsArgumentNullException_WhenIdProviderIsNull()
    {
        // Arrange
        var menuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            MenuEntity.Create(_cafeId, menuName, null!, _clock, sections));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Sync_ReturnsValidationError_WhenNameMissing(string? menuName)
    {
        // Arrange
        var initialMenuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] initialSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        var menu = MenuEntity.Create(_cafeId, initialMenuName, _idProvider, _clock, initialSections).EnsureValue();

        SectionUpdateInfo[] updatedSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        // Act
        var result = menu.SyncMenu(menuName!, updatedSections, _clock, _idProvider);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);

        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuNameRequired, errorDetail.Code);
    }

    [Fact]
    public void Sync_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        var initialMenuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] initialSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        var menu = MenuEntity.Create(_cafeId, initialMenuName, _idProvider, _clock, initialSections).EnsureValue();

        SectionUpdateInfo[] updatedSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.SyncMenu(_faker.Company.CatchPhrase(), updatedSections, null!, _idProvider));
    }

    [Fact]
    public void Sync_ThrowsArgumentNullException_WhenIdProviderIsNull()
    {
        // Arrange
        var initialMenuName = _faker.Company.CatchPhrase();
        SectionUpdateInfo[] initialSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        var menu = MenuEntity.Create(_cafeId, initialMenuName, _idProvider, _clock, initialSections).EnsureValue();

        SectionUpdateInfo[] updatedSections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.SyncMenu(_faker.Company.CatchPhrase(), updatedSections, _clock, null!));
    }

    [Fact]
    public void Publish_ReturnsValidationError_WhenEmptySection()
    {
        // Arrange
        SectionUpdateInfo[] sections = [];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act
        var result = menu.Publish(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);

        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuHasNoSections, errorDetail.Code);
    }

    [Fact]
    public void Publish_ReturnsValidationError_WhenItemsEmpty()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo(itemCount: 0)];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act
        var result = menu.Publish(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);

        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuHasNoItems, errorDetail.Code);
    }

    [Theory]
    [InlineData(MenuState.Deleted, MenuErrorCodes.MenuNotFound)]
    [InlineData(MenuState.Published, MenuErrorCodes.MenuAlreadyPublished)]
    [InlineData(MenuState.Active, MenuErrorCodes.MenuAlreadyActive)]
    public void Publish_ReturnsValidationError_WhenInvalidState(MenuState targetState, string expectedErrorCode)
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        ChangeState(menu, targetState);

        // Act
        var result = menu.Publish(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(expectedErrorCode, errorDetail.Code);
    }

    [Fact]
    public void Publish_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.Publish(null!));
    }

    [Theory]
    [InlineData(MenuState.New)]
    [InlineData(MenuState.Deleted)]
    [InlineData(MenuState.Active)]
    public void Activate_ReturnsError_WhenMenuStateInvalid(MenuState targetState)
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        ChangeState(menu, targetState);

        // Act
        var result = menu.Activate(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuNotPublished, errorDetail.Code);
    }

    [Fact]
    public void Activate_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.Activate(null!));
    }

    [Theory]
    [InlineData(MenuState.New)]
    [InlineData(MenuState.Deleted)]
    [InlineData(MenuState.Published)]
    public void Deactivate_ReturnsValidationError_WhenMenuIsNotActive(MenuState targetState)
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        ChangeState(menu, targetState);

        // Act
        var result = menu.Deactivate(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(MenuErrorCodes.MenuNotActive, errorDetail.Code);
    }

    [Fact]
    public void Deactivate_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.Deactivate(null!));
    }

    [Fact]
    public void SoftDelete_ReturnsConflict_WhenMenuIsActive()
    {
        // Arrange
        var sections = new[] { UpdateInfoDataGenerator.GenerateUpdateSectionInfo() };
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();
        menu.Publish(_clock);
        menu.Activate(_clock);

        // Act
        var result = menu.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error?.Type);
        var error = Assert.Single(result.Error!.Details);
        Assert.Equal(MenuErrorCodes.CannotDeleteActiveMenu, error.Code);
    }

    [Fact]
    public void SoftDelete_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        SectionUpdateInfo[] sections = [UpdateInfoDataGenerator.GenerateUpdateSectionInfo()];
        var menu = MenuEntity.Create(_cafeId, _faker.Company.CatchPhrase(), _idProvider, _clock, sections).EnsureValue();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            menu.SoftDelete(null!));
    }

    private Result ChangeState(MenuEntity menu, MenuState targetState)
    {
        Dictionary<(MenuState, MenuState), Func<MenuEntity, IDateTimeProvider, Result>> stateTransitions = new()
        {
            { (MenuState.New, MenuState.Published), (menu, clock) => menu.Publish(clock) },
            { (MenuState.New, MenuState.Deleted), (menu, clock) => menu.SoftDelete(clock) },
            { (MenuState.New, MenuState.Active), (menu, clock) => {
                var result = menu.Publish(clock);
                return result.IsFailure
                    ? result
                    : menu.Activate(clock);
                }
            },
            { (MenuState.Published, MenuState.Active), (menu, clock) => menu.Activate(clock) },
            { (MenuState.Active, MenuState.Published), (menu, clock) => menu.Deactivate(clock) },
            { (MenuState.Published, MenuState.Deleted), (menu, clock) => menu.SoftDelete(clock) }
        };

        if (menu.State == targetState)
        {
            return Result.Success();
        }

        if (stateTransitions.TryGetValue((menu.State, targetState), out var action))
        {
            return action(menu, _clock);
        }

        throw new InvalidOperationException($"Cannot change state from {menu.State} to {targetState} directly.");
    }
}
