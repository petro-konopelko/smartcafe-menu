using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

namespace SmartCafe.Menu.Infrastructure.Repositories;

public class MenuRepository(MenuDbContext context) : IMenuRepository
{
    public async Task<Domain.Entities.Menu?> GetByIdAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Include(m => m.Sections)
                .ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(m => m.Id == menuId, cancellationToken);
    }

    public async Task<Domain.Entities.Menu?> GetActiveMenuAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Include(m => m.Sections)
                .ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(m => m.CafeId == cafeId && m.State == MenuState.Active, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Domain.Entities.Menu>> GetAllByCafeIdAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Menus
            .Where(m => m.CafeId == cafeId)
            .OrderByDescending(m => m.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Menu> CreateAsync(Domain.Entities.Menu menu, CancellationToken cancellationToken = default)
    {
        await context.Menus.AddAsync(menu, cancellationToken);
        return menu;
    }

    public Task UpdateAsync(Domain.Entities.Menu menu, CancellationToken cancellationToken = default)
    {
        context.Menus.Update(menu);
        return Task.CompletedTask;
    }
}
