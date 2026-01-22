using NSubstitute;
using SmartCafe.Menu.Application.Features.Cafes.CreateCafe;
using SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;
using SmartCafe.Menu.UnitTests.Shared;

namespace SmartCafe.Menu.UnitTests.Application.Cafes;

public class CreateCafeHandlerTests
{
    private readonly ICafeRepository _cafeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly FakeDateTimeProvider _dateTimeProvider;
    private readonly SequenceGuidIdProvider _guidIdProvider;
    private readonly CreateCafeHandler _handler;

    public CreateCafeHandlerTests()
    {
        _cafeRepository = Substitute.For<ICafeRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _dateTimeProvider = new FakeDateTimeProvider();
        _guidIdProvider = new SequenceGuidIdProvider();

        _handler = new CreateCafeHandler(
            _cafeRepository,
            _unitOfWork,
            _eventDispatcher,
            _dateTimeProvider,
            _guidIdProvider);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenCafeIsValid()
    {
        // Arrange
        var cafeName = CafeDataGenerator.GenerateCafeName();
        var contactInfo = "contact@example.com";
        var command = new CreateCafeCommand(cafeName, contactInfo);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.NotEqual(Guid.Empty, response.CafeId);

        await _cafeRepository.Received(1).CreateAsync(Arg.Any<Cafe>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _eventDispatcher.Received(1).DispatchAsync(Arg.Any<IEnumerable<DomainEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateCafeCommand("", null);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();
        Assert.Equal(ErrorType.Validation, error.Type);
    }
}
