namespace SmartCafe.Menu.Domain.Interfaces;

/// <summary>
/// Provides abstraction for DateTime operations to enable testability
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateTime Today { get; }
    DateTimeOffset UtcNowOffset { get; }
    DateTimeOffset NowOffset { get; }
}
