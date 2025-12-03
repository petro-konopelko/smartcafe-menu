namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuUpdatedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId,
    string MenuName
) : DomainEvent(Timestamp, nameof(MenuUpdatedEvent));
