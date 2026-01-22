using NSubstitute;
using SmartCafe.Menu.Application.Features.Cafes.DeleteCafe;
using SmartCafe.Menu.Application.Features.Cafes.DeleteCafe.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.UnitTests.Application.Cafes;

public class DeleteCafeHandlerTests
{
    private readonly ICafeRepository _cafeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly FakeDateTimeProvider _dateTimeProvider;
    private readonly DeleteCafeHandler _handler;

    public DeleteCafeHandlerTests()
    {
        _cafeRepository = Substitute.For<ICafeRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _dateTimeProvider = new FakeDateTimeProvider();

        _handler = new DeleteCafeHandler(
            _cafeRepository,
            _unitOfWork,
            _eventDispatcher,
            _dateTimeProvider);
    }

    [Fact]
    public async Task HandleAsync_DeletesCafeAndDispatchesCafeDeletedEvent_WhenDeletionSucceeds()
    {
        // Arrange
        _dateTimeProvider.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafe = CafeDataGenerator.GenerateValidCafe(_dateTimeProvider);
        cafe.ClearDomainEvents(); // Clear creation event
        var command = new DeleteCafeCommand(cafe.Id);

        _cafeRepository.GetActiveByIdAsync(cafe.Id, Arg.Any<CancellationToken>())
            .Returns(cafe);

        IEnumerable<DomainEvent>? capturedEvents = null;
        await _eventDispatcher.DispatchAsync(
            Arg.Do<IEnumerable<DomainEvent>>(events => capturedEvents = [.. events]),
            Arg.Any<CancellationToken>());

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(cafe.IsDeleted);

        await _cafeRepository.Received(1).UpdateAsync(cafe, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.NotNull(capturedEvents);
        var deletedEvent = Assert.Single(capturedEvents);
        var cafeDeletedEvent = Assert.IsType<CafeDeletedEvent>(deletedEvent);

        Assert.Multiple(() =>
        {
            Assert.Equal(cafe.Id, cafeDeletedEvent.CafeId);
            Assert.Equal(_dateTimeProvider.UtcNow, cafeDeletedEvent.Timestamp);
            Assert.Equal(nameof(CafeDeletedEvent), cafeDeletedEvent.EventType);
        });
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenCafeDoesNotExist()
    {
        // Arrange
        var cafeId = Guid.CreateVersion7();
        var command = new DeleteCafeCommand(cafeId);

        _cafeRepository.GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>())
            .Returns((Cafe?)null);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Multiple(() =>
        {
            Assert.Equal(ErrorType.NotFound, error.Type);
            Assert.Equal(CafeErrorCodes.CafeNotFound, error.Details.First().Code);
        });

        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(TestContext.Current.CancellationToken);
        await _eventDispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task HandleAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        DeleteCafeCommand? command = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _handler.HandleAsync(command!, TestContext.Current.CancellationToken));
    }
}
