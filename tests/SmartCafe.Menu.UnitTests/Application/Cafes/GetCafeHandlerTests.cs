using NSubstitute;
using SmartCafe.Menu.Application.Features.Cafes.GetCafe;
using SmartCafe.Menu.Application.Features.Cafes.GetCafe.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.UnitTests.Application.Cafes;

public class GetCafeHandlerTests
{
    private readonly ICafeRepository _cafeRepository;
    private readonly FakeDateTimeProvider _dateTimeProvider;
    private readonly GetCafeHandler _handler;

    public GetCafeHandlerTests()
    {
        _cafeRepository = Substitute.For<ICafeRepository>();
        _dateTimeProvider = new FakeDateTimeProvider();

        _handler = new GetCafeHandler(_cafeRepository);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenCafeExists()
    {
        // Arrange
        var cafeId = Guid.CreateVersion7();
        var query = new GetCafeQuery(cafeId);

        var cafe = Cafe.Create(
            cafeId,
            "Test Cafe",
            _dateTimeProvider,
            "contact@example.com").EnsureValue();

        _cafeRepository.GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>())
            .Returns(cafe);

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var cafeDto = result.EnsureValue();

        Assert.Multiple(() =>
        {
            Assert.Equal(cafeId, cafeDto.Id);
            Assert.Equal("Test Cafe", cafeDto.Name);
            Assert.Equal("contact@example.com", cafeDto.ContactInfo);
        });

        await _cafeRepository.Received(1).GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenCafeHasNullContactInfo()
    {
        // Arrange
        var cafeId = Guid.CreateVersion7();
        var query = new GetCafeQuery(cafeId);

        var cafe = Cafe.Create(
            cafeId,
            "Test Cafe Without Contact",
            _dateTimeProvider,
            null).EnsureValue();

        _cafeRepository.GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>())
            .Returns(cafe);

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var cafeDto = result.EnsureValue();

        Assert.Multiple(() =>
        {
            Assert.Equal(cafeId, cafeDto.Id);
            Assert.Equal("Test Cafe Without Contact", cafeDto.Name);
            Assert.Null(cafeDto.ContactInfo);
        });

        await _cafeRepository.Received(1).GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFoundError_WhenCafeDoesNotExist()
    {
        // Arrange
        var cafeId = Guid.CreateVersion7();
        var query = new GetCafeQuery(cafeId);

        _cafeRepository.GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>())
            .Returns((Cafe?)null);

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Multiple(() =>
        {
            Assert.Equal(ErrorType.NotFound, error.Type);
            Assert.Contains("not found", error.Details.First().Message, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(CafeErrorCodes.CafeNotFound, error.Details.First().Code);
        });

        await _cafeRepository.Received(1).GetActiveByIdAsync(cafeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        GetCafeQuery? query = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _handler.HandleAsync(query!, TestContext.Current.CancellationToken));
    }
}
