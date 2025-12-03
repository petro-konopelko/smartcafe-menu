using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

public record MenuSummaryDto(
    Guid MenuId,
    string Name,
    MenuState State,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
