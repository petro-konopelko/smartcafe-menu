using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

public record CloneMenuRequest(
    Guid CafeId,
    Guid SourceMenuId,
    string NewMenuName
) : ICommand<Result<CloneMenuResponse>>;
