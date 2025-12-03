using SmartCafe.Menu.Domain.Events;

namespace SmartCafe.Menu.Application.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
}
