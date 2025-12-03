namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuDeletedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId
) : DomainEvent(Timestamp, nameof(MenuDeletedEvent));
