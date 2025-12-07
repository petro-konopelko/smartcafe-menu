using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

public record CloneMenuCommand(
    Guid CafeId,
    Guid SourceMenuId,
    string NewName
) : ICommand<Result<CreateMenuResponse>>;
