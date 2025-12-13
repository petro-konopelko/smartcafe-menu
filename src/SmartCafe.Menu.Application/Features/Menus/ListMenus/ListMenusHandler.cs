using SmartCafe.Menu.Application.Features.Menus.ListMenus.Mappers;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public class ListMenusHandler(IMenuRepository menuRepository) : IQueryHandler<ListMenusQuery, Result<ListMenusResponse>>
{
    public async Task<Result<ListMenusResponse>> HandleAsync(
        ListMenusQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menus = await menuRepository.GetAllByCafeIdAsync(request.CafeId, cancellationToken);

        return Result<ListMenusResponse>.Success(new ListMenusResponse(
            [.. menus.Select(m => m.ToSummaryDto())]
        ));
    }
}
