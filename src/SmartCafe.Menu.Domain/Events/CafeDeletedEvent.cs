namespace SmartCafe.Menu.Domain.Events;

public sealed record CafeDeletedEvent(
    DateTime Timestamp,
    Guid CafeId
) : DomainEvent(Timestamp, nameof(CafeDeletedEvent));
