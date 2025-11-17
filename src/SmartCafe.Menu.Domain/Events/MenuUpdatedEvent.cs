namespace SmartCafe.Menu.Domain.Events;

public record MenuUpdatedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    string MenuName,
    DateTime Timestamp,
    string EventType = "MenuUpdated"
);
