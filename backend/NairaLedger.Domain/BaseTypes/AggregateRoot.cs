using NairaLedger.Domain.DomianEvents;

namespace NairaLedger.Domain.BaseTypes;

/// <summary>
/// Base class for all aggregate roots. Manages a collection of domain events 
/// that must dispatched after persistence.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to be published when the aggregate is persisted.
    /// </summary>
    /// <param name="domainEvent">The event to raise.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes all pending domain events. Called after successful dispatch.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
