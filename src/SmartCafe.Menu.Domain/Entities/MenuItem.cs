using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Domain.Entities;

public class MenuItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid SectionId { get; init; }
    public Section Section { get; init; } = null!;
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Price Price { get; init; }
    public ImageAsset? Image { get; set; }
    public ICollection<MenuItemCategory> MenuItemCategories { get; init; } = [];
    public List<Ingredient> IngredientOptions { get; init; } = [];
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
