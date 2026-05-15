namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a transaction is reversed, either due to a refund, chargeback, or error correction.
/// </summary>
/// <param name="ReversalTransactionId">The unique identifier of the reversal transaction.</param>
/// <param name="OriginalTransactionId">The unique identifier of the original transaction being reversed.</param>
/// <param name="Reason">The reason for the transaction reversal.</param>
public record TransactionReversedEvent(Guid ReversalTransactionId, Guid OriginalTransactionId,  string Reason) : IDomainEvent
{  
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
