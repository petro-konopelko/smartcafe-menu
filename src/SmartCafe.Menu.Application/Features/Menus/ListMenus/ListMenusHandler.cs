using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public class ListMenusHandler(IMenuRepository menuRepository) : IQueryHandler<ListMenusQuery, Result<ListMenusResponse>>
{
    public async Task<Result<ListMenusResponse>> HandleAsync(
        ListMenusQuery request,
        CancellationToken cancellationToken = default)
    {
        var menus = await menuRepository.GetAllByCafeIdAsync(request.CafeId, cancellationToken);

        return Result<ListMenusResponse>.Success(new ListMenusResponse(
            menus.Select(m => new MenuSummaryDto(
                m.Id,
                m.Name,
                m.IsActive,
                m.IsPublished,
                m.CreatedAt,
                m.UpdatedAt
            )).ToList()
        ));
    }
}
