using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

public record ListMenusQuery(
    Guid CafeId
) : IQuery<Result<ListMenusResponse>>;
