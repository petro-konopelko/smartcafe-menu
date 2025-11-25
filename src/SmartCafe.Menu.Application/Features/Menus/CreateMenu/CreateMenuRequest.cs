using SmartCafe.Menu.Application.Features.Menus.Shared;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public record CreateMenuRequest(
    Guid CafeId,
    string Name,
    List<SectionDto> Sections
) : ICommand<CreateMenuResponse>;
