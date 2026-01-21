using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class DeleteMenuHandlerTests
{
    private readonly ICafeRepository _cafeRepository = Substitute.For<ICafeRepository>();
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public async Task HandleAsync_DeletesMenuImagesAndDispatches_WhenDeletionSucceeds()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var menu = MenuTestData.CreatePublishedMenu(cafeId, new SequenceGuidIdProvider(), _clock);

        menuRepository.GetByIdAsync(menu.Id, Arg.Any<CancellationToken>()).Returns(menu);
        _cafeRepository.ExistsActiveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new DeleteMenuHandler(menuRepository, _cafeRepository, imageStorageService, unitOfWork, eventDispatcher, _clock);
        var command = new DeleteMenuCommand(cafeId, menu.Id);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);

        await menuRepository.Received(1).UpdateAsync(menu, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await imageStorageService.Received(1).DeleteMenuImagesAsync(cafeId, menu.Id, Arg.Any<CancellationToken>());
        await eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenMenuDoesNotExist()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var menuRepository = Substitute.For<IMenuRepository>();
        var imageStorageService = Substitute.For<IImageStorageService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        var menuId = Guid.NewGuid();
        menuRepository.GetByIdAsync(menuId, Arg.Any<CancellationToken>()).Returns((MenuEntity?)null);
        _cafeRepository.ExistsActiveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var handler = new DeleteMenuHandler(menuRepository, _cafeRepository, imageStorageService, unitOfWork, eventDispatcher, _clock);
        var command = new DeleteMenuCommand(cafeId, menuId);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(MenuErrorCodes.MenuNotFound, Assert.Single(error.Details).Code);

        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await imageStorageService.DidNotReceiveWithAnyArgs().DeleteMenuImagesAsync(default, default, ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }
}
