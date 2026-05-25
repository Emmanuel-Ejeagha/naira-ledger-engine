namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when an incoming Paystack webhook passes signature verification.
/// </summary>
/// <param name="WebhookId">The unique identifier of the webhook that was verified.</param>
/// <param name="EventId">The unique identifier of the event associated with the webhook.</param>
public record WebhookVerifiedEvent(Guid WebhookId, string EventId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
