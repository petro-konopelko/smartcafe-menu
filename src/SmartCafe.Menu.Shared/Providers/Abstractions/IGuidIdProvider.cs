namespace SmartCafe.Menu.Shared.Providers.Abstractions;

/// <summary>
/// Provides time-ordered GUIDs for entity IDs.
/// Allows testability by abstracting GUID generation.
/// </summary>
public interface IGuidIdProvider
{
    /// <summary>
    /// Generates a new time-ordered GUID (version 7).
    /// </summary>
    Guid NewId();
}
