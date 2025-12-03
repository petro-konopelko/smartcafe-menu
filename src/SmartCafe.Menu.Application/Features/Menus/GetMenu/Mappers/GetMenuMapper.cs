using GetModels = SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Mappers;

public static class GetMenuMapper
{
    public static GetModels.GetMenuResponse ToGetMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new GetModels.GetMenuResponse(
            menu.Id,
            menu.Name,
            menu.State,
            menu.Sections.Select(s => new GetModels.MenuSectionDto(
                s.Id,
                s.Name,
                s.AvailableFrom,
                s.AvailableTo,
                s.DisplayOrder,
                s.Items.Select(i => new GetModels.MenuItemDto(
                    i.Id,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.ImageBigUrl,
                    i.ImageCroppedUrl,
                    i.IsActive,
                    i.MenuItemCategories.Select(c => c.CategoryId).ToList(),
                    i.IngredientOptions.Select(ing => new GetModels.IngredientDto(
                        ing.Name,
                        ing.IsExcludable,
                        ing.IsIncludable
                    )).ToList()
                )).ToList()
            )).ToList(),
            menu.CreatedAt,
            menu.UpdatedAt
        );
    }
}
