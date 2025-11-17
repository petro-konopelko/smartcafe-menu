namespace SmartCafe.Menu.Application.Features.Menus.ListMenus;

public record MenuSummaryDto(
    Guid MenuId,
    string Name,
    bool IsActive,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
