using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public class GetActiveMenuHandler(IMenuRepository menuRepository) : IQueryHandler<GetActiveMenuQuery, Result<GetActiveMenuResponse>>
{
    public async Task<Result<GetActiveMenuResponse>> HandleAsync(
        GetActiveMenuQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetActiveMenuAsync(request.CafeId, cancellationToken);

        if (menu == null)
        {
            return Result<GetActiveMenuResponse>.Failure(Error.NotFound(
                $"No active menu found for cafe {request.CafeId}",
                ErrorCodes.MenuNotFound));
        }

        return Result<GetActiveMenuResponse>.Success(menu.ToGetActiveMenuResponse());
    }
}
