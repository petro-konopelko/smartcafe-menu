using SmartCafe.Menu.Application.Features.Menus.Shared;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public record UpdateMenuRequest(
    string Name,
    List<SectionDto> Sections
);
