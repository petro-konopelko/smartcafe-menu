namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

public record CloneMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    bool IsActive,
    bool IsPublished,
    DateTime CreatedAt
);
