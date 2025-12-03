namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuDeactivatedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId
) : DomainEvent(Timestamp, nameof(MenuDeactivatedEvent));
