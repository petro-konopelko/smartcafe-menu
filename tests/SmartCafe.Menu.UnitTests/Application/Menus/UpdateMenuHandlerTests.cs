using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class UpdateMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    [Fact]
    public async Task HandleAsync_SavesAndDispatches_WhenUpdateValid()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var menu = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new UpdateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock, _idProvider);
        var command = MenuTestData.CreateUpdateMenuCommandFromMenu(menu, cafeId, newName: "Updated name");

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);
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

        var handler = new UpdateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock, _idProvider);
        var command = MenuTestData.CreateValidUpdateMenuCommand(cafeId: cafeId, menuId: menuId);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);

        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenMenuBelongsToAnotherCafe()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var expectedCafeId = Guid.NewGuid();
        var otherCafeId = Guid.NewGuid();

        var menu = MenuTestData.CreateNewMenu(otherCafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);

        var handler = new UpdateMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock, _idProvider);
        var command = MenuTestData.CreateValidUpdateMenuCommand(cafeId: expectedCafeId, menuId: menu.Id);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);

        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }
}
