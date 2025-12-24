using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.UnitTests.Shared;

public class DateTimeProviderTests
{
    [Fact]
    public void FakeDateTimeProvider_Should_FreezeTime()
    {
        // Arrange
        var provider = new FakeDateTimeProvider();
        var fixedTime = new DateTime(2025, 11, 16, 10, 30, 0, DateTimeKind.Utc);

        // Act
        provider.SetUtcNow(fixedTime);

        // Assert
        Assert.Equal(fixedTime, provider.UtcNow);
        Assert.Equal(DateTimeKind.Utc, provider.UtcNow.Kind);
    }

    [Fact]
    public void FakeDateTimeProvider_Should_ReturnLocalTime()
    {
        // Arrange
        var provider = new FakeDateTimeProvider();
        var fixedUtcTime = new DateTime(2025, 11, 16, 10, 30, 0, DateTimeKind.Utc);
        provider.SetUtcNow(fixedUtcTime);

        // Act
        var localTime = provider.Now;

        // Assert
        Assert.Equal(fixedUtcTime.ToLocalTime(), localTime);
    }
}
