using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
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
        Assert.False(cafe.IsDeleted);
        Assert.Null(cafe.UpdatedAt);
    }

    [Fact]
    public void Create_ReturnsCafe_WithContactInfo()
    {
        // Arrange
        _clock.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafeId = Guid.NewGuid();
        var cafeName = CafeDataGenerator.GenerateCafeName();
        var contactInfo = "contact@example.com";

        // Act
        var result = Cafe.Create(cafeId, cafeName, _clock, contactInfo);

        // Assert
        Assert.True(result.IsSuccess);
        var cafe = result.EnsureValue();

        Assert.Equal(cafeId, cafe.Id);
        Assert.Equal(cafeName, cafe.Name);
        Assert.Equal(contactInfo, cafe.ContactInfo);
        Assert.Equal(_clock.UtcNow, cafe.CreatedAt);
    }

    [Fact]
    public void Create_RaisesCafeCreatedEvent()
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

        var domainEvent = Assert.Single(cafe.DomainEvents);
        var cafeCreatedEvent = Assert.IsType<CafeCreatedEvent>(domainEvent);
        Assert.Equal(cafeId, cafeCreatedEvent.CafeId);
        Assert.Equal(cafeName, cafeCreatedEvent.CafeName);
        Assert.Equal(_clock.UtcNow, cafeCreatedEvent.Timestamp);
    }

    [Fact]
    public void SoftDelete_MarksCafeAsDeleted()
    {
        // Arrange
        _clock.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafe = CafeDataGenerator.GenerateValidCafe(_clock);
        var originalCreatedAt = cafe.CreatedAt;
        _clock.SetUtcNow(new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));

        // Act
        var result = cafe.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(cafe.IsDeleted);
        Assert.Equal(_clock.UtcNow, cafe.UpdatedAt);
        Assert.Equal(originalCreatedAt, cafe.CreatedAt);
    }

    [Fact]
    public void SoftDelete_RaisesCafeDeletedEvent()
    {
        // Arrange
        _clock.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafe = CafeDataGenerator.GenerateValidCafe(_clock);
        cafe.ClearDomainEvents();
        _clock.SetUtcNow(new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));

        // Act
        var result = cafe.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsSuccess);
        var domainEvent = Assert.Single(cafe.DomainEvents);
        var cafeDeletedEvent = Assert.IsType<CafeDeletedEvent>(domainEvent);
        Assert.Equal(cafe.Id, cafeDeletedEvent.CafeId);
        Assert.Equal(_clock.UtcNow, cafeDeletedEvent.Timestamp);
    }

    [Theory]
    [InlineData("  ", CafeErrorCodes.CafeNameRequired)]
    [InlineData("", CafeErrorCodes.CafeNameRequired)]
    public void Create_ReturnsValidationError_WhenNameInvalid(string name, string expectedCode)
    {
        // Act
        var result = Cafe.Create(Guid.NewGuid(), name, _clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(expectedCode, errorDetail.Code);
    }

    [Fact]
    public void Create_ReturnsValidationError_WhenContactInfoTooLong()
    {
        // Arrange
        var contactInfo = new string('a', Cafe.MaxContactInfoLength + 1);

        // Act
        var result = Cafe.Create(Guid.NewGuid(), CafeDataGenerator.GenerateCafeName(), _clock, contactInfo);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(CafeErrorCodes.CafeContactInfoTooLong, errorDetail.Code);
    }

    [Fact]
    public void Create_ReturnsValidationError_WhenNameTooLong()
    {
        // Arrange
        var longName = new string('a', Cafe.MaxNameLength + 1);

        // Act
        var result = Cafe.Create(Guid.NewGuid(), longName, _clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Validation, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(CafeErrorCodes.CafeNameTooLong, errorDetail.Code);
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

    [Fact]
    public void SoftDelete_ReturnsConflict_WhenAlreadyDeleted()
    {
        // Arrange
        var cafe = CafeDataGenerator.GenerateValidCafe(_clock);
        cafe.SoftDelete(_clock);

        // Act
        var result = cafe.SoftDelete(_clock);

        // Assert
        Assert.True(result.IsFailure);
        var error = result.EnsureError();

        Assert.Equal(ErrorType.Conflict, error.Type);
        var errorDetail = Assert.Single(error.Details);
        Assert.Equal(CafeErrorCodes.CafeAlreadyDeleted, errorDetail.Code);
    }

    [Fact]
    public void SoftDelete_ThrowsArgumentNullException_WhenClockIsNull()
    {
        // Arrange
        var cafe = CafeDataGenerator.GenerateValidCafe(_clock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => cafe.SoftDelete(null!));
    }
}
