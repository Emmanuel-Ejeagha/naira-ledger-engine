using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a new wallet is created for a user.
/// </summary>
/// <param name="WalletId">The unique identifier of the newly created wallet.</param>
/// <param name="UserId">The unique identifier of the user for whom the wallet is created.</param>
public record WalletCreatedEvent(Guid WalletId, UserId UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
