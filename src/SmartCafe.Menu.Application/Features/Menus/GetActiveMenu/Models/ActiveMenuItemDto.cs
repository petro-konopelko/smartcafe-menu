namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record ActiveMenuItemDto(
    string Name,
    string? Description,
    decimal Price,
    string? ImageBigUrl,
    string? ImageCroppedUrl,
    List<Guid> CategoryIds,
    List<ActiveIngredientDto> Ingredients
);
