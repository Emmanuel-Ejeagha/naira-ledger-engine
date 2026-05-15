using NairaLedger.Domain.Enums;

namespace NairaLedger.Domain.DomianEvents;

public record KycVerifiedEvent(Guid WalletId, KycLevel NewLevel) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
