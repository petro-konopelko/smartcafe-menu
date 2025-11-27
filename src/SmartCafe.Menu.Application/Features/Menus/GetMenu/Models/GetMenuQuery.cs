using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record GetMenuQuery(
    Guid CafeId,
    Guid MenuId
) : IQuery<GetMenuResponse>;
