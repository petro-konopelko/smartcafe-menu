namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record MenuItemDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    ImageItemDto? Image,
    bool IsActive,
    List<Guid> CategoryIds,
    List<IngredientDto> Ingredients
);
