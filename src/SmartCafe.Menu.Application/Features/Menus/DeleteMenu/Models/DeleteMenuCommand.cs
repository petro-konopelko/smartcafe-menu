using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;

public record DeleteMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result>;
