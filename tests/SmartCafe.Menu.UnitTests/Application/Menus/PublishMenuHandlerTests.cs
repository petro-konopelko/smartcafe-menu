using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class PublishMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_PublishesMenu_WhenMenuInNewState()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var menu = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new PublishMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock);
        var command = new PublishMenuCommand(cafeId, menu.Id);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();

        Assert.Multiple(
            () => Assert.Equal(menu.Id, response.MenuId),
            () => Assert.Equal(menu.Name, response.MenuName),
            () => Assert.Equal(menu.PublishedAt, response.PublishedAt));

        await menuRepository.Received(1).UpdateAsync(menu, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenMenuDoesNotExist()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        menuRepository.GetByIdAsync(menuId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);

        var handler = new PublishMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock);
        var command = new PublishMenuCommand(cafeId, menuId);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);

        await menuRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, ct);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }
}
