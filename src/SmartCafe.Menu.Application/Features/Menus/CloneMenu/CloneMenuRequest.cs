using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public record CloneMenuRequest(
    Guid CafeId,
    Guid SourceMenuId,
    string NewMenuName
) : ICommand<CloneMenuResponse>;
