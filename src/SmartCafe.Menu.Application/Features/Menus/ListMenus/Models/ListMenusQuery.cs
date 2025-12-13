using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

public record ListMenusQuery(
    Guid CafeId
) : IQuery<Result<ListMenusResponse>>;
