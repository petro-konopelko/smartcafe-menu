using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

public record CloneMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    MenuState State,
    DateTime CreatedAt
);
