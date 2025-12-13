using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;

public record ActivateMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result<ActivateMenuResponse>>;
