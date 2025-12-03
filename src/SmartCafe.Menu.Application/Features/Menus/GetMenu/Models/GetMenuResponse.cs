using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record GetMenuResponse(
    Guid MenuId,
    string Name,
    MenuState State,
    List<MenuSectionDto> Sections,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
