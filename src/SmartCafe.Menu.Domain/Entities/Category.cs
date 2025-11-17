namespace SmartCafe.Menu.Domain.Entities;

public class Category
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public string? IconUrl { get; set; }
    public bool IsDefault { get; init; }
    public ICollection<MenuItemCategory> MenuItemCategories { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}
