using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;

public record UpdateMenuCommand(
    Guid CafeId,
    Guid MenuId,
    string Name,
    List<SectionDto> Sections
) : ICommand<Result>;
