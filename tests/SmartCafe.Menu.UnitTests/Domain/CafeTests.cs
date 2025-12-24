using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Tests.Shared.DataGenerators;
using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.UnitTests.Domain;

public class CafeTests
{
    private readonly FakeDateTimeProvider _clock = new();

    [Fact]
    public void Create_ReturnsCafe_WhenInputIsValid()
    {
        // Arrange
        _clock.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafeId = Guid.NewGuid();
        var cafeName = CafeDataGenerator.GenerateCafeName();

        // Act
        var result = Cafe.Create(cafeId, cafeName, _clock);

        // Assert
        Assert.True(result.IsSuccess);
        var cafe = result.EnsureValue();

        Assert.Equal(cafeId, cafe.Id);
        Assert.Equal(cafeName, cafe.Name);
        Assert.Equal(_clock.UtcNow, cafe.CreatedAt);
    }

    [Theory]
    [InlineData("  ", CafeErrorCodes.CafeNameRequired)]
    [InlineData("", CafeErrorCodes.CafeNameRequired)]
    public void Create_ReturnsValidationError_WhenNameInvalid(string name, string expectedCode)
    {
        // Act
        var result = Cafe.Create(Guid.NewGuid(), name, _clock);

        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(expectedCode, errorDetail.Code);
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Cafe.Create(Guid.Empty, CafeDataGenerator.GenerateCafeName(), _clock));
    }

    [Fact]
    public void Create_ThrowsArgumentNullException_WhenClockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Cafe.Create(Guid.NewGuid(), CafeDataGenerator.GenerateCafeName(), null!));
    }
}
