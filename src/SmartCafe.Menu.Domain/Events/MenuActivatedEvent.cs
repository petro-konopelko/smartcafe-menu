namespace SmartCafe.Menu.Domain.Events;

public record MenuActivatedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    string MenuName,
    DateTime Timestamp,
    string EventType = "MenuActivated"
);
