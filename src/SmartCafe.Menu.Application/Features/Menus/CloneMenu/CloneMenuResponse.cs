namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public record CloneMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    bool IsActive,
    bool IsPublished,
    DateTime CreatedAt
);
