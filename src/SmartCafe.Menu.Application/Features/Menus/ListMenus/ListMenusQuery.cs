using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public record ListMenusQuery(
    Guid CafeId
) : IQuery<ListMenusResponse>;

public record ListMenusResponse(
    List<MenuSummaryDto> Menus
);
