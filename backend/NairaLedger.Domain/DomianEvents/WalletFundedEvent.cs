namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a wallet is funded (credit).
/// </summary>
/// <param name="TransactionId">The unique identifier of the transaction.</param>
/// <param name="WalletId">The unique identifier of the wallet.</param>
/// <param name="Amount">The amount credited to the wallet.</param>
public record WalletFundedEvent(Guid TransactionId, Guid WalletId, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
