using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

public record ListMenusQuery(
    Guid CafeId
) : IQuery<Result<ListMenusResponse>>;
