namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised after a fraud velocity check identifies a suspicious pattern.
/// </summary>
/// <param name="WalletId">The unique identifier of the wallet being checked for fraud.</param>
/// <param name="RuleName">The name of the fraud detection rule that was triggered.</param>
/// <param name="Description">A description of the suspicious activity detected.</param>
public record FraudCheckTriggeredEvent(Guid WalletId, string RuleName, string Description) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
