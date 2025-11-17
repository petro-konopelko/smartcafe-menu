using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Categories.ListCategories;

public class ListCategoriesHandler(ICategoryRepository categoryRepository)
{
    public async Task<List<CategoryDto>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        return categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.IconUrl,
            c.IsDefault
        )).ToList();
    }
}
