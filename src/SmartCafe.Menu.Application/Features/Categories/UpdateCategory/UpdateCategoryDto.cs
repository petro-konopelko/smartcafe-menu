namespace SmartCafe.Menu.Application.Features.Categories.UpdateCategory;

public record UpdateCategoryRequest(
    string Name,
    string? IconUrl
);

public record CategoryDto(
    Guid Id,
    string Name,
    string? Icon,
    bool IsDefault
);
