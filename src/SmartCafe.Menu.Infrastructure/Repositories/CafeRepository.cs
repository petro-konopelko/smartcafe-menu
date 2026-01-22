using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;
using CafeEntity = SmartCafe.Menu.Domain.Entities.Cafe;

namespace SmartCafe.Menu.Infrastructure.Repositories;

public class CafeRepository(MenuDbContext context) : ICafeRepository
{
    public async Task<bool> ExistsActiveAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Cafes.AnyAsync(c => c.Id == cafeId && !c.IsDeleted, cancellationToken);
    }

    public async Task<CafeEntity?> GetActiveByIdAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Cafes
            .FirstOrDefaultAsync(c => c.Id == cafeId && !c.IsDeleted, cancellationToken);
    }

    public async Task<List<CafeEntity>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.Cafes
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<CafeEntity> CreateAsync(CafeEntity cafe, CancellationToken cancellationToken = default)
    {
        await context.Cafes.AddAsync(cafe, cancellationToken);
        return cafe;
    }

    public Task UpdateAsync(CafeEntity cafe, CancellationToken cancellationToken = default)
    {
        context.Cafes.Update(cafe);
        return Task.CompletedTask;
    }
}
