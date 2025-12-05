using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu;

public class GetMenuHandler(IMenuRepository menuRepository) : IQueryHandler<GetMenuQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> HandleAsync(
        GetMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        return menu is null || menu.CafeId != request.CafeId
            ? Result<MenuDto>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound))
            : Result<MenuDto>.Success(menu.ToMenuDto());
    }
}
