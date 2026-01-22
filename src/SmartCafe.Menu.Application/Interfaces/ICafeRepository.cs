using CafeEntity = SmartCafe.Menu.Domain.Entities.Cafe;

namespace SmartCafe.Menu.Application.Interfaces;

public interface ICafeRepository
{
    Task<bool> ExistsActiveAsync(Guid cafeId, CancellationToken cancellationToken = default);
    Task<CafeEntity?> GetActiveByIdAsync(Guid cafeId, CancellationToken cancellationToken = default);
    Task<List<CafeEntity>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<CafeEntity> CreateAsync(CafeEntity cafe, CancellationToken cancellationToken = default);
    Task UpdateAsync(CafeEntity cafe, CancellationToken cancellationToken = default);
}
