namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a P2P transfer fails, either due to insufficient funds, KYC issues, or other validation errors. 
/// </summary>
/// <param name="TransactionId">The unique identifier of the transaction.</param>
/// <param name="FromWalletId">The unique identifier of the wallet from which the funds were debited.</param>
/// <param name="ToWalletId">The unique identifier of the wallet to which the funds were intended to be credited.</param>
/// <param name="Amount">The amount attempted to be transferred.</param>
/// <param name="Reason">The reason for the transfer failure.</param>
public record TransferFailedEvent(Guid TransactionId, Guid FromWalletId, Guid ToWalletId, decimal Amount, string Reason) : IDomainEvent
{    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
