using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;

public record GetActiveMenuQuery(
    Guid CafeId
) : IQuery<GetActiveMenuResponse>;
