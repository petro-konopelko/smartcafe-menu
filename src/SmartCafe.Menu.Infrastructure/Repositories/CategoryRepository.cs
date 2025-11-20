using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

namespace SmartCafe.Menu.Infrastructure.Repositories;

public class CategoryRepository(MenuDbContext context, IDateTimeProvider dateTimeProvider) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Categories.FindAsync([categoryId], cancellationToken);
    }

    public async Task<List<Category>> GetByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default)
    {
        // Convert to HashSet for O(1) Contains performance
        var categoryIdSet = categoryIds.ToHashSet();
        return await context.Categories
            .Where(c => categoryIdSet.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        category.GetType().GetProperty(nameof(Category.CreatedAt))!.SetValue(category, dateTimeProvider.UtcNow);
        context.Categories.Add(category);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Update(category);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Remove(category);
        await Task.CompletedTask;
    }
}
