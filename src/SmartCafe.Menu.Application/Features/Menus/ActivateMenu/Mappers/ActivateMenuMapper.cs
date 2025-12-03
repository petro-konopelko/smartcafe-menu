using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Mappers;

public static class ActivateMenuMapper
{
    public static ActivateMenuResponse ToActivateMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new ActivateMenuResponse(menu.Id, menu.Name, menu.ActivatedAt ?? menu.UpdatedAt);
    }
}
