using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record MenuItemDto(
    Guid? Id,
    string Name,
    string? Description,
    decimal Price,
    List<Guid> CategoryIds,
    List<Ingredient> IngredientOptions
);
