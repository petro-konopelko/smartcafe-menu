namespace SmartCafe.Menu.Domain.Events;

public record MenuClonedEvent(
    Guid EventId,
    Guid OriginalMenuId,
    Guid NewMenuId,
    Guid CafeId,
    string NewMenuName,
    DateTime Timestamp,
    string EventType = "MenuCloned"
);
