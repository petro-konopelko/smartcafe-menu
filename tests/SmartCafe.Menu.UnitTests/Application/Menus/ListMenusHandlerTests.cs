using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.ListMenus;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class ListMenusHandlerTests
{
    private readonly ICafeRepository _cafeRepository = Substitute.For<ICafeRepository>();
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_ReturnsMenusForCafe()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();

        var cafeId = Guid.NewGuid();

        var menu1 = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock, name: "Menu 1");
        var menu2 = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock, name: "Menu 2");

        menuRepository.GetAllByCafeIdAsync(cafeId, Arg.Any<CancellationToken>()).Returns(new[] { menu1, menu2 });
        _cafeRepository.ExistsActiveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new ListMenusHandler(menuRepository, _cafeRepository);
        var query = new ListMenusQuery(cafeId);

        // Act
        var result = await handler.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();

        Assert.Equal(2, response.Menus.Count);
        Assert.Contains(response.Menus, m => m.MenuId == menu1.Id && m.Name == "Menu 1");
        Assert.Contains(response.Menus, m => m.MenuId == menu2.Id && m.Name == "Menu 2");
    }
}
