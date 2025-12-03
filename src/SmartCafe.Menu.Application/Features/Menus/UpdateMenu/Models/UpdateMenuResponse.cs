using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;

public record UpdateMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    MenuState State,
    DateTime UpdatedAt
);
