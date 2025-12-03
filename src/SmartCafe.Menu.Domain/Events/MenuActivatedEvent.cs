namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuActivatedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId,
    string MenuName
) : DomainEvent(Timestamp, nameof(MenuActivatedEvent));
