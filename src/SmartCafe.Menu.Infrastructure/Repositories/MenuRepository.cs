using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

namespace SmartCafe.Menu.Infrastructure.Repositories;

public class MenuRepository(MenuDbContext context, IDateTimeProvider dateTimeProvider) : IMenuRepository
{
    public async Task<Domain.Entities.Menu?> GetByIdAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Include(m => m.Sections)
                .ThenInclude(s => s.Items)
                    .ThenInclude(i => i.MenuItemCategories)
                        .ThenInclude(mic => mic.Category)
            .FirstOrDefaultAsync(m => m.Id == menuId, cancellationToken);
    }

    public async Task<Domain.Entities.Menu?> GetActiveMenuAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Include(m => m.Sections)
                .ThenInclude(s => s.Items)
                    .ThenInclude(i => i.MenuItemCategories)
                        .ThenInclude(mic => mic.Category)
            .FirstOrDefaultAsync(m => m.CafeId == cafeId && m.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Menu>> GetAllByCafeIdAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Where(m => m.CafeId == cafeId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Menu> CreateAsync(Domain.Entities.Menu menu, CancellationToken cancellationToken = default)
    {
        var now = dateTimeProvider.UtcNow;

        // Set timestamps using IDateTimeProvider via reflection since properties are init-only
        menu.GetType().GetProperty(nameof(Domain.Entities.Menu.CreatedAt))!.SetValue(menu, now);
        menu.GetType().GetProperty(nameof(Domain.Entities.Menu.UpdatedAt))!.SetValue(menu, now);

        context.Menus.Add(menu);
        return menu;
    }

    public async Task UpdateAsync(Domain.Entities.Menu menu, CancellationToken cancellationToken = default)
    {
        menu.UpdatedAt = dateTimeProvider.UtcNow;
        context.Menus.Update(menu);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Domain.Entities.Menu menu, CancellationToken cancellationToken = default)
    {
        menu.IsDeleted = true;
        menu.UpdatedAt = dateTimeProvider.UtcNow;
        context.Menus.Update(menu);
        await Task.CompletedTask;
    }
}
