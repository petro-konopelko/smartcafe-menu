namespace SmartCafe.Menu.Domain.Events;

public abstract record DomainEvent(
    DateTime Timestamp,
    string EventType
)
{
    public Guid EventId { get; } = Guid.NewGuid();
}
