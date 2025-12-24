using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.GetMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class GetMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_ReturnsMenuDto_WhenMenuFound()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();

        imageStorageService.GetAbsoluteUrl(Arg.Any<string>()).Returns(callInfo => "https://cdn.test/" + callInfo.Arg<string>());

        var cafeId = Guid.NewGuid();
        var menu = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new GetMenuHandler(menuRepository, imageStorageService);
        var query = MenuTestData.CreateGetMenuQuery(cafeId: cafeId, menuId: menu.Id);

        // Act
        var result = await handler.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var dto = result.EnsureValue();

        Assert.Multiple(
            () => Assert.Equal(menu.Id, dto.Id),
            () => Assert.Equal(menu.Name, dto.Name),
            () => Assert.Equal(menu.State, dto.State));
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenMenuBelongsToAnotherCafe()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();

        var expectedCafeId = Guid.NewGuid();
        var otherCafeId = Guid.NewGuid();

        var menu = MenuTestData.CreateNewMenu(otherCafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new GetMenuHandler(menuRepository, imageStorageService);
        var query = MenuTestData.CreateGetMenuQuery(cafeId: expectedCafeId, menuId: menu.Id);

        // Act
        var result = await handler.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenMenuNotFound()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();

        var cafeId = Guid.NewGuid();
        var menuId = Guid.NewGuid();

        menuRepository.GetByIdAsync(menuId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);

        var handler = new GetMenuHandler(menuRepository, imageStorageService);
        var query = MenuTestData.CreateGetMenuQuery(cafeId: cafeId, menuId: menuId);

        // Act
        var result = await handler.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);
    }
}
