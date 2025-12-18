namespace SmartCafe.Menu.Domain.Models;

public record SectionUpdateInfo(
    Guid? Id,
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    IReadOnlyCollection<ItemUpdateInfo> Items
) : IUpdateInfoIdentity
{ }
