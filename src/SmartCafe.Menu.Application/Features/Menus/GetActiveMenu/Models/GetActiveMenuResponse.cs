namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record GetActiveMenuResponse(
    Guid MenuId,
    string Name,
    List<ActiveMenuSectionDto> Sections
);
