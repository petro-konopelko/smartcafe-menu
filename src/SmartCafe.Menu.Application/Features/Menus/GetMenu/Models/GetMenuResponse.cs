namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record GetMenuResponse(
    Guid MenuId,
    string Name,
    bool IsActive,
    bool IsPublished,
    List<MenuSectionDto> Sections,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
