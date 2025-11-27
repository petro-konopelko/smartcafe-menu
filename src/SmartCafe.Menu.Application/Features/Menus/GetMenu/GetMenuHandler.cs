using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu;

public class GetMenuHandler(IMenuRepository menuRepository) : IQueryHandler<GetMenuQuery, Result<GetMenuResponse>>
{
    public async Task<Result<GetMenuResponse>> HandleAsync(
        GetMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result<GetMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        return Result<GetMenuResponse>.Success(new GetMenuResponse(
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
        ));
    }
}
