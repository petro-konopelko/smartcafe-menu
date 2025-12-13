namespace SmartCafe.Menu.Shared.Providers.Abstractions;

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
