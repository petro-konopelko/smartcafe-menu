namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public record GetActiveMenuResponse(
    Guid MenuId,
    string Name,
    List<ActiveMenuSectionDto> Sections
);

public record ActiveMenuSectionDto(
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    int DisplayOrder,
    List<ActiveMenuItemDto> Items
);

public record ActiveMenuItemDto(
    string Name,
    string? Description,
    decimal Price,
    string? ImageBigUrl,
    string? ImageCroppedUrl,
    List<Guid> CategoryIds,
    List<ActiveIngredientDto> Ingredients
);

public record ActiveIngredientDto(
    string Name,
    bool IsExcludable,
    bool IsIncludable
);
