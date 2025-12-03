using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Mappers;

public static class GetActiveMenuMapper
{
    public static GetActiveMenuResponse ToGetActiveMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new GetActiveMenuResponse(
            menu.Id,
            menu.Name,
            menu.Sections.Select(s => new ActiveMenuSectionDto(
                s.Name,
                s.AvailableFrom,
                s.AvailableTo,
                s.DisplayOrder,
                s.Items.Where(i => i.IsActive).Select(i => new ActiveMenuItemDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.ImageBigUrl,
                    i.ImageCroppedUrl,
                    i.MenuItemCategories.Select(c => c.CategoryId).ToList(),
                    i.IngredientOptions.Select(ing => new ActiveIngredientDto(
                        ing.Name,
                        ing.IsExcludable,
                        ing.IsIncludable
                    )).ToList()
                )).ToList()
            )).ToList()
        );
    }
}
