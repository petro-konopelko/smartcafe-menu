namespace SmartCafe.Menu.Application.Features.Categories.ListCategories;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Icon,
    bool IsDefault
);
