using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public record CreateMenuRequest(
    string Name,
    List<SectionDto> Sections
);

public record SectionDto(
    string Name,
    int DisplayOrder,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    List<MenuItemDto> Items
);

public record MenuItemDto(
    string Name,
    string Description,
    decimal Price,
    List<Guid> CategoryIds,
    List<Ingredient> IngredientOptions
);
