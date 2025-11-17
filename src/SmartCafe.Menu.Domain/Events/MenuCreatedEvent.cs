namespace SmartCafe.Menu.Domain.Events;

public record MenuCreatedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    string MenuName,
    DateTime Timestamp,
    string EventType = "MenuCreated"
);
