using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Events;

namespace SmartCafe.Menu.Infrastructure.EventBus;

public class DomainEventDispatcher(IEventPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var @event in events)
        {
            // Publish as object to avoid generic constraints at runtime
            await publisher.PublishAsync(@event, cancellationToken: cancellationToken);
        }
    }
}
