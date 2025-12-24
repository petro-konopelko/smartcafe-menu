using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Events;

namespace SmartCafe.Menu.IntegrationTests.Mocks;

public sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
