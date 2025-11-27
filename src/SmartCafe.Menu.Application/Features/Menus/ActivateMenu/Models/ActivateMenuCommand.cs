using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;

public record ActivateMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result<ActivateMenuResponse>>;
