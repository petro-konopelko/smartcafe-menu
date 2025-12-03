using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Mappers;

public static class ListMenusMapper
{
    public static MenuSummaryDto ToSummaryDto(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new MenuSummaryDto(
            menu.Id,
            menu.Name,
            menu.State,
            menu.CreatedAt,
            menu.UpdatedAt
        );
    }
}
