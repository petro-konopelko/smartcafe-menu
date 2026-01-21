using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu;

public class GetMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
    IImageStorageService imageStorageService) : IQueryHandler<GetMenuQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> HandleAsync(
        GetMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists (not deleted)
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<MenuDto>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        return menu is null || menu.CafeId != request.CafeId
            ? Result<MenuDto>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                MenuErrorCodes.MenuNotFound))
            : Result<MenuDto>.Success(menu.ToMenuDto(imageStorageService));
    }
}
