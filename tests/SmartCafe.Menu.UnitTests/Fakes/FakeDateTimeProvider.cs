using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.UnitTests.Fakes;

/// <summary>
/// Fake implementation of IDateTimeProvider for testing
/// Allows freezing time to test time-dependent logic
/// </summary>
public class FakeDateTimeProvider : IDateTimeProvider
{
    private DateTime _utcNow = DateTime.UtcNow;
    
    public DateTime UtcNow => _utcNow;
    public DateTime Now => _utcNow.ToLocalTime();
    public DateTime Today => _utcNow.Date;
    public DateTimeOffset UtcNowOffset => new DateTimeOffset(_utcNow);
    public DateTimeOffset NowOffset => new DateTimeOffset(_utcNow.ToLocalTime());
    
    /// <summary>
    /// Set the current UTC time for testing
    /// </summary>
    public void SetUtcNow(DateTime utcNow)
    {
        _utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }
    
    /// <summary>
    /// Reset to actual current time
    /// </summary>
    public void Reset()
    {
        _utcNow = DateTime.UtcNow;
    }
}
