namespace SmartCafe.Menu.Domain.Models;

public sealed class ItemUpdateInfo(
    Guid? id,
    string name,
    string? description,
    PriceUpdateInfo price,
    ImageUpdateInfo? image,
    IReadOnlyCollection<IngredientItemUpdate> ingredients
) : IUpdateInfoIdentity
{
    public Guid? Id => id;
    public string Name => name;
    public string? Description => description;
    public PriceUpdateInfo Price => price;
    public ImageUpdateInfo? Image => image;
    public IReadOnlyCollection<IngredientItemUpdate> Ingredients => ingredients;
}
