using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu;

public class GetMenuHandler(IMenuRepository menuRepository)
{
    public async Task<GetMenuResponse?> HandleAsync(
        Guid cafeId,
        Guid menuId,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(menuId, cancellationToken);

        if (menu == null || menu.CafeId != cafeId)
        {
            return null;
        }

        return new GetMenuResponse(
            menu.Id,
            menu.Name,
            menu.IsActive,
            menu.IsPublished,
            menu.Sections.Select(s => new MenuSectionDto(
                s.Id,
                s.Name,
                s.AvailableFrom,
                s.AvailableTo,
                s.DisplayOrder,
                s.Items.Select(i => new MenuItemDto(
                    i.Id,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.ImageBigUrl,
                    i.ImageCroppedUrl,
                    i.IsActive,
                    i.MenuItemCategories.Select(c => c.CategoryId).ToList(),
                    i.IngredientOptions.Select(ing => new IngredientDto(
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
