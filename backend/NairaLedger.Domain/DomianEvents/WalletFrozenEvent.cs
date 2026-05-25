namespace NairaWallet.Domain.DomainEvents;

/// <summary>
/// Raised when a wallet is frozen by the fraud detection system.
/// </summary>
public record WalletFrozenEvent(Guid WalletId, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}