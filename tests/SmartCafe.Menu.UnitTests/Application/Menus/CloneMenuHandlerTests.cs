using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class CloneMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_CreatesClonedMenu_WhenSourceMenuFound()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var sourceMenu = MenuTestData.CreateNewMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(sourceMenu.Id, Arg.Any<CancellationToken>()).Returns(sourceMenu);

        var idProvider = new SequenceGuidIdProvider();
        var expectedClonedMenuId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var handler = new CloneMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock, idProvider);
        var command = MenuTestData.CreateCloneMenuCommand(cafeId: cafeId, sourceMenuId: sourceMenu.Id, newName: "Cloned menu");

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.Multiple(
            () => Assert.Equal(expectedClonedMenuId, response.MenuId),
            () => Assert.Equal(cafeId, response.CafeId));

        await menuRepository.Received(1).CreateAsync(Arg.Is<MenuEntity>(m => m.Id == expectedClonedMenuId && m.CafeId == cafeId), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenSourceMenuDoesNotExist()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var sourceMenuId = Guid.NewGuid();

        menuRepository.GetByIdAsync(sourceMenuId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);

        var handler = new CloneMenuHandler(menuRepository, unitOfWork, eventDispatcher, _clock, new SequenceGuidIdProvider());
        var command = MenuTestData.CreateCloneMenuCommand(cafeId: cafeId, sourceMenuId: sourceMenuId);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);

        await menuRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, ct);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }
}
