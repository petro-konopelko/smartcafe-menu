using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Shared.Providers;

/// <summary>
/// Production implementation of IGuidIdProvider using Guid.CreateVersion7().
/// </summary>
public class GuidIdProvider : IGuidIdProvider
{
    public Guid NewId() => Guid.CreateVersion7();
}
