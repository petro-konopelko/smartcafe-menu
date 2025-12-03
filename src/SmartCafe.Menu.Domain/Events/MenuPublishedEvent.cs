namespace SmartCafe.Menu.Domain.Events;

public sealed record MenuPublishedEvent(
    DateTime Timestamp,
    Guid MenuId,
    Guid CafeId,
    string MenuName
) : DomainEvent(Timestamp, nameof(MenuPublishedEvent));
