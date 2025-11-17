namespace SmartCafe.Menu.Domain.Events;

public record MenuDeletedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    DateTime Timestamp,
    string EventType = "MenuDeleted"
);
