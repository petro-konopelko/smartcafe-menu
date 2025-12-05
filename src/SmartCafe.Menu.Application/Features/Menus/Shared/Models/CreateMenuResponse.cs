namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record CreateMenuResponse(
    Guid MenuId,
    Guid CafeId
);
