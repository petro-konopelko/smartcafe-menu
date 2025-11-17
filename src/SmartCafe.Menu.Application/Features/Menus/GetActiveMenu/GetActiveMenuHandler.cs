using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public class GetActiveMenuHandler(IMenuRepository menuRepository)
{
    public async Task<GetActiveMenuResponse?> HandleAsync(
        Guid cafeId,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetActiveMenuAsync(cafeId, cancellationToken);

        if (menu == null)
        {
            return null;
        }

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
