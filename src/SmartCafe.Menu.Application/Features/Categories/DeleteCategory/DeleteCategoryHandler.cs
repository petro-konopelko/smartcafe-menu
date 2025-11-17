using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        if (category.IsDefault)
        {
            throw new InvalidOperationException("Cannot delete default categories");
        }

        // Check if category is in use
        if (category.MenuItemCategories.Any())
        {
            throw new InvalidOperationException("Cannot delete category that is assigned to menu items");
        }

        await categoryRepository.DeleteAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
