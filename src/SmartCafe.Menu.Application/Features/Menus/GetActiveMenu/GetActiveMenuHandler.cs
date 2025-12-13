using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public class GetActiveMenuHandler(IMenuRepository menuRepository, IImageStorageService imageStorageService) : IQueryHandler<GetActiveMenuQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> HandleAsync(
        GetActiveMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetActiveMenuAsync(request.CafeId, cancellationToken);

        return menu is null
            ? Result<MenuDto>.Failure(Error.NotFound(
                $"No active menu found for cafe {request.CafeId}",
                MenuErrorCodes.MenuNotFound))
            : Result<MenuDto>.Success(menu.ToMenuDto(imageStorageService));
    }
}
