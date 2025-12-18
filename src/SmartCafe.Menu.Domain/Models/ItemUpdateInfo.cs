namespace SmartCafe.Menu.Domain.Models;

public record ItemUpdateInfo(
    Guid? Id,
    string Name,
    string? Description,
    PriceUpdateInfo Price,
    ImageUpdateInfo? Image,
    IReadOnlyCollection<IngredientItemUpdate> Ingredients
) : IUpdateInfoIdentity
{ }
