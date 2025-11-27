namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;

public record UpdateMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    bool IsActive,
    bool IsPublished,
    DateTime UpdatedAt
);
