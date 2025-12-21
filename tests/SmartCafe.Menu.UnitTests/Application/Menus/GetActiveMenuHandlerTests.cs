using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class GetActiveMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_ReturnsActiveMenuDto_WhenFound()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();

        imageStorageService.GetAbsoluteUrl(Arg.Any<string>()).Returns(callInfo => "https://cdn.test/" + callInfo.Arg<string>());

        var cafeId = Guid.NewGuid();
        var menu = MenuTestData.CreateActiveMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetActiveMenuAsync(cafeId, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new GetActiveMenuHandler(menuRepository, imageStorageService);
        var query = MenuTestData.CreateGetActiveMenuQuery(cafeId);

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
    public async Task HandleAsync_ReturnsNotFound_WhenNoActiveMenu()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();

        var cafeId = Guid.NewGuid();
        menuRepository.GetActiveMenuAsync(cafeId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);

        var handler = new GetActiveMenuHandler(menuRepository, imageStorageService);
        var query = MenuTestData.CreateGetActiveMenuQuery(cafeId);

        // Act
        var result = await handler.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);
    }
}
