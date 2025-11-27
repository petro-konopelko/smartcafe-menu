using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;

public record ActivateMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result<ActivateMenuResponse>>;
