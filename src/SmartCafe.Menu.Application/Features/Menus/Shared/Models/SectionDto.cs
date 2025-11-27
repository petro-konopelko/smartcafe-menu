namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record SectionDto(
    Guid? Id,
    string Name,
    int DisplayOrder,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    List<MenuItemDto> Items
);
