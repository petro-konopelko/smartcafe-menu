using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

public record CloneMenuCommand(
    Guid CafeId,
    Guid SourceMenuId,
    string NewName
) : ICommand<Result<CreateMenuResponse>>;
