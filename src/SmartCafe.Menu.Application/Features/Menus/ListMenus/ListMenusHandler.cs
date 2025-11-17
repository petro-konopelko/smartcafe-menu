using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public class ListMenusHandler(IMenuRepository menuRepository)
{
    public async Task<List<MenuSummaryDto>> HandleAsync(
        Guid cafeId,
        CancellationToken cancellationToken = default)
    {
        var menus = await menuRepository.GetAllByCafeIdAsync(cafeId, cancellationToken);

        return menus.Select(m => new MenuSummaryDto(
            m.Id,
            m.Name,
            m.IsActive,
            m.IsPublished,
            m.CreatedAt,
            m.UpdatedAt
        )).ToList();
    }
}
