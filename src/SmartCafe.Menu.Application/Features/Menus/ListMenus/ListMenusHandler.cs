using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public class ListMenusHandler(IMenuRepository menuRepository) : IQueryHandler<ListMenusQuery, ListMenusResponse>
{
    public async Task<ListMenusResponse> HandleAsync(
        ListMenusQuery request,
        CancellationToken cancellationToken = default)
    {
        var menus = await menuRepository.GetAllByCafeIdAsync(request.CafeId, cancellationToken);

        return new ListMenusResponse(
            menus.Select(m => new MenuSummaryDto(
                m.Id,
                m.Name,
                m.IsActive,
                m.IsPublished,
                m.CreatedAt,
                m.UpdatedAt
            )).ToList()
        );
    }
}
