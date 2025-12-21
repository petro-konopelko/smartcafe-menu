using NSubstitute;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.UnitTests.Fakes;
using SmartCafe.Menu.UnitTests.Shared;

using DomainEvent = SmartCafe.Menu.Domain.Events.DomainEvent;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Application.Menus;

public class CreateMenuHandlerTests
{
    private readonly FakeDateTimeProvider _clock = new();
    private readonly SequenceGuidIdProvider _idProvider = new();

    [Fact]
    public async Task HandleAsync_CreatesMenuAndDispatchesEvents_WhenRequestValid()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var cafeRepository = Substitute.For<ICafeRepository>();
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        cafeRepository.ExistsAsync(cafeId, Arg.Any<CancellationToken>()).Returns(true);

        var expectedMenuId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var createdAt = new DateTime(2025, 12, 21, 12, 0, 0, DateTimeKind.Utc);
        _clock.SetUtcNow(createdAt);

        var handler = new CreateMenuHandler(
            cafeRepository,
            menuRepository,
            unitOfWork,
            eventDispatcher,
            _clock,
            _idProvider);

        var command = MenuTestData.CreateValidCreateMenuCommand(cafeId: cafeId, sectionCount: 2, itemsPerSection: 2);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.Multiple(
            () => Assert.Equal(expectedMenuId, response.MenuId),
            () => Assert.Equal(cafeId, response.CafeId));

        await menuRepository.Received(1).CreateAsync(Arg.Is<MenuEntity>(m => m.Id == expectedMenuId && m.CafeId == cafeId), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenCafeDoesNotExist()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var cafeRepository = Substitute.For<ICafeRepository>();
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        cafeRepository.ExistsAsync(cafeId, Arg.Any<CancellationToken>()).Returns(false);

        var handler = new CreateMenuHandler(
            cafeRepository,
            menuRepository,
            unitOfWork,
            eventDispatcher,
            _clock,
            _idProvider);

        var command = MenuTestData.CreateValidCreateMenuCommand(cafeId: cafeId);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(CafeErrorCodes.CafeNotFound, Assert.Single(error.Details).Code);

        await menuRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, ct);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_ReturnsValidation_WhenDomainRejectsMenuName(string name)
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        var cafeRepository = Substitute.For<ICafeRepository>();
        var menuRepository = Substitute.For<IMenuRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var eventDispatcher = Substitute.For<IDomainEventDispatcher>();

        var cafeId = Guid.NewGuid();
        cafeRepository.ExistsAsync(cafeId, Arg.Any<CancellationToken>()).Returns(true);

        var handler = new CreateMenuHandler(
            cafeRepository,
            menuRepository,
            unitOfWork,
            eventDispatcher,
            _clock,
            _idProvider);

        var command = MenuTestData.CreateValidCreateMenuCommand(cafeId: cafeId, name: name);

        // Act
        var result = await handler.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);

        await menuRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!, ct);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(ct);
        await eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, ct);
    }
}
