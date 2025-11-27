using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public class GetActiveMenuHandler(IMenuRepository menuRepository) : IQueryHandler<GetActiveMenuQuery, Result<GetActiveMenuResponse>>
{
    public async Task<Result<GetActiveMenuResponse>> HandleAsync(
        GetActiveMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetActiveMenuAsync(request.CafeId, cancellationToken);

        if (menu == null)
        {
            return Result<GetActiveMenuResponse>.Failure(Error.NotFound(
                $"No active menu found for cafe {request.CafeId}",
                "ACTIVE_MENU_NOT_FOUND"));
        }

        return Result<GetActiveMenuResponse>.Success(new GetActiveMenuResponse(
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
        ));
    }
}
