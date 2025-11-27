namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record IngredientDto(
    string Name,
    bool IsExcludable,
    bool IsIncludable
);
