using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Categories.CreateCategory;

public class CreateCategoryHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<CategoryDto> HandleAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            IconUrl = request.IconUrl,
            IsDefault = false
        };

        category.GetType().GetProperty(nameof(Category.CreatedAt))!
            .SetValue(category, dateTimeProvider.UtcNow);

        await categoryRepository.CreateAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.IconUrl, category.IsDefault);
    }
}
