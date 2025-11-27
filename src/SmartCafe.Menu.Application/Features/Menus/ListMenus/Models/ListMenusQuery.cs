using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

public record ListMenusQuery(
    Guid CafeId
) : IQuery<ListMenusResponse>;
