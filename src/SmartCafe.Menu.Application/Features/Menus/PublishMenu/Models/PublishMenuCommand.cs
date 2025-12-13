using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;

public record PublishMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result<PublishMenuResponse>>;
