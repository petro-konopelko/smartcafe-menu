using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.API.Models.Requests;

public record UpdateMenuRequest(
    string Name,
    List<SectionDto> Sections
);
