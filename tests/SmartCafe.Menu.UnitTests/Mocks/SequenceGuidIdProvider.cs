using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.UnitTests.Shared;

/// <summary>
/// Test helper that provides deterministic GUIDs in sequence for predictable test execution.
/// </summary>
public sealed class SequenceGuidIdProvider : IGuidIdProvider
{
    private readonly Queue<Guid> _ids;

    public SequenceGuidIdProvider()
    {
        _ids = new Queue<Guid>(
            Enumerable
            .Range(1, 100)
            .Select(index => Guid.Parse($"00000000-0000-0000-0000-{index:D12}")));
    }

    public SequenceGuidIdProvider(IEnumerable<Guid> ids)
    {
        _ids = new Queue<Guid>(ids);
    }

    public Guid NewId()
    {
        return _ids.Count > 0 ? _ids.Dequeue() : Guid.NewGuid();
    }
}
