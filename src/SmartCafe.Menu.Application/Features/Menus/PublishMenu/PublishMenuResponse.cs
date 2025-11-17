namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public record PublishMenuResponse(
    Guid MenuId,
    string MenuName,
    DateTime PublishedAt
);
