namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record MenuItemDto(
    Guid ItemId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageBigUrl,
    string? ImageCroppedUrl,
    bool IsActive,
    List<Guid> CategoryIds,
    List<IngredientDto> Ingredients
);
