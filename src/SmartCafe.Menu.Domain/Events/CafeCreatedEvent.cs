namespace SmartCafe.Menu.Domain.Events;

public sealed record CafeCreatedEvent(
    DateTime Timestamp,
    Guid CafeId,
    string CafeName
) : DomainEvent(Timestamp, nameof(CafeCreatedEvent));
