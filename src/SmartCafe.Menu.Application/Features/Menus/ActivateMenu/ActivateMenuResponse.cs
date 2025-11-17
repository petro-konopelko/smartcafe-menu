namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

public record ActivateMenuResponse(
    Guid MenuId,
    string MenuName,
    DateTime ActivatedAt
);
