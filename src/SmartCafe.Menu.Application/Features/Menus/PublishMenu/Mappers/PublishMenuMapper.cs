using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu.Mappers;

public static class PublishMenuMapper
{
    public static PublishMenuResponse ToPublishMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new PublishMenuResponse(menu.Id, menu.Name, menu.PublishedAt ?? menu.UpdatedAt);
    }
}
