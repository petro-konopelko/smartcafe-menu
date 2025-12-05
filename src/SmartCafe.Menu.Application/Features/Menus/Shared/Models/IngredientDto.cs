namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record IngredientDto(
    string Name,
    bool IsExcludable,
    bool IsIncludable
);
