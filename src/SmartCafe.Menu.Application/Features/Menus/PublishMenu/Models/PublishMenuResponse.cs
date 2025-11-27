namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;

public record PublishMenuResponse(
    Guid MenuId,
    string MenuName,
    DateTime PublishedAt
);
