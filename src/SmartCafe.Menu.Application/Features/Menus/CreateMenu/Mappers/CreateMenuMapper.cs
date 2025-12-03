using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Mappers;

public static class CreateMenuMapper
{
    public static CreateMenuResponse ToCreateMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new CreateMenuResponse(
            menu.Id,
            menu.CafeId,
            menu.Name,
            menu.State,
            menu.CreatedAt
        );
    }
}
