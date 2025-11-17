using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CategoryDto> HandleAsync(
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        if (category.IsDefault)
        {
            throw new InvalidOperationException("Cannot modify default categories");
        }

        // Create updated category (Name is init-only)
        var updatedCategory = new Domain.Entities.Category
        {
            Id = category.Id,
            Name = request.Name,
            IconUrl = request.IconUrl,
            IsDefault = category.IsDefault
        };

        updatedCategory.GetType().GetProperty(nameof(Domain.Entities.Category.CreatedAt))!
            .SetValue(updatedCategory, category.CreatedAt);

        await categoryRepository.UpdateAsync(updatedCategory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(updatedCategory.Id, updatedCategory.Name, updatedCategory.IconUrl, updatedCategory.IsDefault);
    }
}
