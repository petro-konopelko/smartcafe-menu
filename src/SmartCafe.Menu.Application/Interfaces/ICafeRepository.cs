namespace SmartCafe.Menu.Application.Interfaces;

public interface ICafeRepository
{
    Task<bool> ExistsAsync(Guid cafeId, CancellationToken cancellationToken = default);
}
