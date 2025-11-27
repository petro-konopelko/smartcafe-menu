using Microsoft.EntityFrameworkCore;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Infrastructure.Data.PostgreSQL;

namespace SmartCafe.Menu.Infrastructure.Repositories;

public class CafeRepository(MenuDbContext context) : ICafeRepository
{
    public async Task<bool> ExistsAsync(Guid cafeId, CancellationToken cancellationToken = default)
    {
        return await context.Cafes.AnyAsync(c => c.Id == cafeId, cancellationToken);
    }
}
