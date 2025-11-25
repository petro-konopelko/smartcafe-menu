using SmartCafe.Menu.Application.Features.Menus.Shared;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public record UpdateMenuRequest(
    Guid CafeId,
    Guid MenuId,
    string Name,
    List<SectionDto> Sections
) : ICommand<UpdateMenuResponse>;
