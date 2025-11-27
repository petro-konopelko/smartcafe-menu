using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record GetMenuQuery(
    Guid CafeId,
    Guid MenuId
) : IQuery<Result<GetMenuResponse>>;
