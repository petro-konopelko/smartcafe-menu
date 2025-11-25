using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

public record ActivateMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<ActivateMenuResponse>;
