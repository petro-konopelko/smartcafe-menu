using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Mappers;

public static class CloneMenuMapper
{
    public static CloneMenuResponse ToCloneMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new CloneMenuResponse(menu.Id, menu.CafeId, menu.Name, menu.State, menu.CreatedAt);
    }
}
