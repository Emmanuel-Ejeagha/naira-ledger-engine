namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a P2P transfer completes successfully.
/// </summary>
/// <param name="TransactionId">The unique identifier of the transaction.</param>
/// <param name="FromWalletId">The unique identifier of the wallet from which the funds were debited.</param>
/// <param name="ToWalletId">The unique identifier of the wallet to which the funds were credited.</param>
/// <param name="Amount">The amount transferred.</param>
public record TransferCompletedEvent(Guid TransactionId, Guid FromWalletId, Guid ToWalletId, decimal Amount) : IDomainEvent
{    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
