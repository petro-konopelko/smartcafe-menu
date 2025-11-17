namespace SmartCafe.Menu.Domain.Entities;

public class MenuItemCategory
{
    public Guid MenuItemId { get; init; }
    public MenuItem MenuItem { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public Category Category { get; init; } = null!;
}
