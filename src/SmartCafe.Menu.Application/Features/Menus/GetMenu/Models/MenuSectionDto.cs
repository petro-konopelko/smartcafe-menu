namespace SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;

public record MenuSectionDto(
    Guid SectionId,
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    int DisplayOrder,
    List<MenuItemDto> Items
);
