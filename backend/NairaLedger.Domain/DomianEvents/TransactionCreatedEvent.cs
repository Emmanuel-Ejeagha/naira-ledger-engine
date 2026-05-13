namespace NairaLedger.Domain.DomianEvents;

public record TransactionCreatedEvent(Guid TransactionId, Guid WalletId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
