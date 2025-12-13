namespace SmartCafe.Menu.Domain.Models;

public sealed class SectionUpdateInfo(
    Guid? id,
    string name,
    TimeSpan? availableFrom,
    TimeSpan? availableTo,
    IReadOnlyCollection<ItemUpdateInfo> items
) : IUpdateInfoIdentity
{
    public Guid? Id => id;
    public string Name => name;
    public TimeSpan? AvailableFrom => availableFrom;
    public TimeSpan? AvailableTo => availableTo;
    public IReadOnlyCollection<ItemUpdateInfo> Items => items;
}
