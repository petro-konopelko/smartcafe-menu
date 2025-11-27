namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record ActiveIngredientDto(
    string Name,
    bool IsExcludable,
    bool IsIncludable
);
