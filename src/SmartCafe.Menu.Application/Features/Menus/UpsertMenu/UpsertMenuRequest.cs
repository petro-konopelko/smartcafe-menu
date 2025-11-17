namespace SmartCafe.Menu.Application.Features.Menus.UpsertMenu;

public record UpsertMenuRequest(
    Guid? MenuId, // Null for create, Guid for update
    string Name,
    string? Description,
    List<UpsertSectionDto> Sections
);

public record UpsertSectionDto(
    Guid? SectionId, // Null for new sections
    string Name,
    string? Description,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    int DisplayOrder,
    List<UpsertMenuItemDto> Items
);

public record UpsertMenuItemDto(
    Guid? ItemId, // Null for new items
    string Name,
    string? Description,
    decimal Price,
    string? BigImageUrl,
    string? CroppedImageUrl,
    bool IsAvailable,
    List<Guid> CategoryIds,
    List<IngredientDto> Ingredients
);

public record IngredientDto(
    string Name,
    bool CanBeExcluded,
    bool CanBeIncluded
);
