using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Interfaces;

public interface IMenuRepository
{
    Task<MenuEntity?> GetByIdAsync(Guid menuId, CancellationToken cancellationToken = default);
    Task<MenuEntity?> GetActiveMenuAsync(Guid cafeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuEntity>> GetAllByCafeIdAsync(Guid cafeId, CancellationToken cancellationToken = default);
    Task<MenuEntity> CreateAsync(MenuEntity menu, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuEntity menu, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuEntity menu, CancellationToken cancellationToken = default);
}
