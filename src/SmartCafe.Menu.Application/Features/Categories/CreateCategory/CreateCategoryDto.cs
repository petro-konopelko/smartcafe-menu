namespace SmartCafe.Menu.Application.Features.Categories.CreateCategory;

public record CreateCategoryRequest(
    string Name,
    string? IconUrl
);

public record CategoryDto(
    Guid Id,
    string Name,
    string? Icon,
    bool IsDefault
);
