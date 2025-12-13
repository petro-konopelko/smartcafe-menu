namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record MenuItemDto(
    Guid? Id,
    string Name,
    string? Description,
    PriceDto Price,
    MenuItemImageDto? Image,
    IReadOnlyCollection<IngredientDto> Ingredients
);
