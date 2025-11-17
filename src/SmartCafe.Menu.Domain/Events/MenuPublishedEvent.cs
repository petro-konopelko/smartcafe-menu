namespace SmartCafe.Menu.Domain.Events;

public record MenuPublishedEvent(
    Guid EventId,
    Guid MenuId,
    Guid CafeId,
    string MenuName,
    DateTime Timestamp,
    string EventType = "MenuPublished"
);
