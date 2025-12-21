using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class ActivateMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_DeactivatesCurrentMenuAndActivatesTarget_WhenValid()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var currentActiveMenu = MenuTestData.CreateActiveMenu(cafeId, new SequenceGuidIdProvider(), _clock);
        var targetMenu = MenuTestData.CreatePublishedMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(targetMenu.Id, Arg.Any<CancellationToken>()).Returns(targetMenu);
        menuRepository.GetActiveMenuAsync(cafeId, Arg.Any<CancellationToken>()).Returns(currentActiveMenu);

        var handler = new ActivateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock);
        var command = new ActivateMenuCommand(cafeId, targetMenu.Id);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();

        Assert.Multiple(
            () => Assert.Equal(targetMenu.Id, response.MenuId),
            () => Assert.Equal(targetMenu.Name, response.MenuName),
            () => Assert.Equal(targetMenu.ActivatedAt, response.ActivatedAt));

        await menuRepository.Received(1).UpdateAsync(currentActiveMenu, Arg.Any<CancellationToken>());
        await menuRepository.Received(1).UpdateAsync(targetMenu, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsConflict_WhenTargetMenuIsNotPublished()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var targetMenu = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(targetMenu.Id, Arg.Any<CancellationToken>()).Returns(targetMenu);
        menuRepository.GetActiveMenuAsync(cafeId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);

        var handler = new ActivateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock);
        var command = new ActivateMenuCommand(cafeId, targetMenu.Id);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(ErrorType.Conflict, error.Type);
        Assert.Equal(MenuErrorCodes.MenuNotPublished, Assert.Single(error.Details).Code);

        await menuRepository.DidNotReceive().UpdateAsync(targetMenu, Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
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

        var handler = new ActivateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock);
        var command = new ActivateMenuCommand(cafeId, menuId);

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
