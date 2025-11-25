using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public record PublishMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<PublishMenuResponse>;
