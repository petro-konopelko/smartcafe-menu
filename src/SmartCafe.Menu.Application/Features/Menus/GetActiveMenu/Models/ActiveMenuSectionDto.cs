namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record ActiveMenuSectionDto(
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    int DisplayOrder,
    List<ActiveMenuItemDto> Items
);
