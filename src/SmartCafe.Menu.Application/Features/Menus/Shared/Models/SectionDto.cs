namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record SectionDto(
    Guid? Id,
    string Name,
    TimeSpan? AvailableFrom,
    TimeSpan? AvailableTo,
    IReadOnlyCollection<MenuItemDto> Items
);
