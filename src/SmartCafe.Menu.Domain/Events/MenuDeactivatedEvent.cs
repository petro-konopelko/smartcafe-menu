namespace SmartCafe.Menu.Domain.Events;

public record MenuDeactivatedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    DateTime Timestamp,
    string EventType = "MenuDeactivated"
);
