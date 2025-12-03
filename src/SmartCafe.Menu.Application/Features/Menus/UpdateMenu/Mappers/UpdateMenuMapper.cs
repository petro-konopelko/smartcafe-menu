using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Mappers;

public static class UpdateMenuMapper
{
    public static UpdateMenuResponse ToUpdateMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new UpdateMenuResponse(menu.Id, menu.CafeId, menu.Name, menu.State, menu.UpdatedAt);
    }
}
