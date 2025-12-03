using SmartCafe.Menu.Domain.Events;

namespace SmartCafe.Menu.Domain.Common;

/// <summary>
/// Base entity providing domain event collection mechanics.
/// </summary>
public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
