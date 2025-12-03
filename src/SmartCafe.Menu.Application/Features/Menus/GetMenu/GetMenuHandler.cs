using SmartCafe.Menu.Application.Features.Menus.GetMenu.Mappers;
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
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result<GetMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        return Result<GetMenuResponse>.Success(menu.ToGetMenuResponse());
    }
}
