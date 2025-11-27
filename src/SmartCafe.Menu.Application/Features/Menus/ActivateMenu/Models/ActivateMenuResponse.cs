namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;

public record ActivateMenuResponse(
    Guid MenuId,
    string MenuName,
    DateTime ActivatedAt
);
