using SmartCafe.Menu.Application.Features.Menus.Shared;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public record CreateMenuRequest(
    string Name,
    List<SectionDto> Sections
);
