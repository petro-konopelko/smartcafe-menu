using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared;

public record SectionDto(
    Guid? Id,
    string Name,
    int DisplayOrder,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    List<MenuItemDto> Items
);

public record MenuItemDto(
    Guid? Id,
    string Name,
    string? Description,
    decimal Price,
    List<Guid> CategoryIds,
    List<Ingredient> IngredientOptions
);
