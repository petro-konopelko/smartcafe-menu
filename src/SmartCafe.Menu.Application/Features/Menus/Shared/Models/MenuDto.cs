using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record MenuDto(
    Guid Id,
    string Name,
    MenuState State,
    List<SectionDto> Sections,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
