using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;

public record CreateMenuResponse(
    Guid Id,
    Guid CafeId,
    string Name,
    MenuState State,
    DateTime CreatedAt
);
