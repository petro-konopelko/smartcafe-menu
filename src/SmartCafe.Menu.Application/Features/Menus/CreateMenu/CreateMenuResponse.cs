namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public record CreateMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    bool IsActive,
    bool IsPublished,
    DateTime CreatedAt
);
