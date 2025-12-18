using Bogus;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.UnitTests.Fakes;

namespace SmartCafe.Menu.UnitTests.Domain;

public class CafeTests
{
    private readonly FakeDateTimeProvider _clock = new();
    private readonly Faker _faker = new();

    [Fact]
    public void Create_ReturnsCafe_WhenInputIsValid()
    {
        _clock.SetUtcNow(new DateTime(2024, 1, 1, 8, 30, 0, DateTimeKind.Utc));
        var cafeId = Guid.NewGuid();
        var cafeName = _faker.Company.CompanyName();

        var result = Cafe.Create(cafeId, cafeName, _clock);

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
        var result = Cafe.Create(Guid.NewGuid(), name, _clock);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        var error = Assert.Single(result.Error!.Details);
        Assert.Equal(expectedCode, error.Code);
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Cafe.Create(Guid.Empty, _faker.Company.CompanyName(), _clock));
    }

    [Fact]
    public void Create_ThrowsArgumentNullException_WhenClockIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Cafe.Create(Guid.NewGuid(), _faker.Company.CompanyName(), null!));
    }
}
