using NSubstitute;
using SmartCafe.Menu.Application.Features.Cafes.ListCafes;
using SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.UnitTests.Application.Cafes;

public class ListCafesHandlerTests
{
    private readonly ICafeRepository _cafeRepository;
    private readonly FakeDateTimeProvider _dateTimeProvider;
    private readonly ListCafesHandler _handler;

    public ListCafesHandlerTests()
    {
        _cafeRepository = Substitute.For<ICafeRepository>();
        _dateTimeProvider = new FakeDateTimeProvider();

        _handler = new ListCafesHandler(_cafeRepository);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyList_WhenNoCafesExist()
    {
        // Arrange
        var query = new ListCafesQuery();
        _cafeRepository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.Empty(response.Cafes);

        await _cafeRepository.Received(1).GetAllActiveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsCafeList_WhenCafesExist()
    {
        // Arrange
        var query = new ListCafesQuery();
        var cafeId1 = Guid.CreateVersion7();
        var cafeId2 = Guid.CreateVersion7();
        var cafeId3 = Guid.CreateVersion7();

        var cafe1 = Cafe.Create(cafeId1, "Cafe One", _dateTimeProvider, "contact1@example.com").EnsureValue();
        var cafe2 = Cafe.Create(cafeId2, "Cafe Two", _dateTimeProvider, "contact2@example.com").EnsureValue();
        var cafe3 = Cafe.Create(cafeId3, "Cafe Three", _dateTimeProvider, null).EnsureValue();

        _cafeRepository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Cafe> { cafe1, cafe2, cafe3 });

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.Equal(3, response.Cafes.Count);

        var expectedCafes = new[]
        {
            (cafe1, "Cafe One", "contact1@example.com"),
            (cafe2, "Cafe Two", "contact2@example.com"),
            (cafe3, "Cafe Three", null)
        };

        Assert.All(response.Cafes, (dto, index) =>
        {
            var expected = expectedCafes[index];
            Assert.Multiple(() =>
            {
                Assert.Equal(expected.Item1.Id, dto.Id);
                Assert.Equal(expected.Item2, dto.Name);
                Assert.Equal(expected.Item3, dto.ContactInfo);
                Assert.Equal(expected.Item1.CreatedAt, dto.CreatedAt);
                Assert.Null(dto.UpdatedAt);
            });
        });

        await _cafeRepository.Received(1).GetAllActiveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_OnlyReturnsActiveCafes_DeletedCafesAreFiltered()
    {
        // Arrange
        var query = new ListCafesQuery();
        var activeCafeId = Guid.CreateVersion7();
        var activeCafe = Cafe.Create(activeCafeId, "Active Cafe", _dateTimeProvider, "active@example.com").EnsureValue();

        // Repository should only return active cafes (deleted ones already filtered)
        _cafeRepository.GetAllActiveAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Cafe> { activeCafe });

        // Act
        var result = await _handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.EnsureValue();
        Assert.Single(response.Cafes);
        Assert.Equal(activeCafe.Id, response.Cafes[0].Id);

        await _cafeRepository.Received(1).GetAllActiveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.HandleAsync(null!, TestContext.Current.CancellationToken));
    }
}
