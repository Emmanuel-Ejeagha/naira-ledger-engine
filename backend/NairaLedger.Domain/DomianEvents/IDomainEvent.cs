namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Marker interface for all domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// UTC timestamp when the event occured.
    /// </summary>
    DateTime OccurredOn { get; }
}
