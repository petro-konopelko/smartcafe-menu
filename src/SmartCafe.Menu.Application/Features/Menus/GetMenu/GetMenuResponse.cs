namespace SmartCafe.Menu.Application.Features.Menus.GetMenu;

public record GetMenuResponse(
    Guid MenuId,
    string Name,
    bool IsActive,
    bool IsPublished,
    List<MenuSectionDto> Sections,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MenuSectionDto(
    Guid SectionId,
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    int DisplayOrder,
    List<MenuItemDto> Items
);

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

public record IngredientDto(
    string Name,
    bool IsExcludable,
    bool IsIncludable
);
