namespace SmartCafe.Menu.Application.Features.Menus.UpsertMenu;

public record UpsertMenuResponse(
    Guid MenuId,
    string Name,
    string? Description,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
