namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuClonedEvent(
    DateTime Timestamp,
    Guid OriginalMenuId,
    Guid NewMenuId,
    Guid CafeId,
    string NewMenuName
) : DomainEvent(Timestamp, nameof(MenuClonedEvent));
