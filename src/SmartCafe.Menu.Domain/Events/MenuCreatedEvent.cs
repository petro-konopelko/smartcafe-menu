namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuCreatedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId,
    string MenuName
) : DomainEvent(Timestamp, nameof(MenuCreatedEvent));
