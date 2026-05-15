namespace NairaWallet.Application.Interfaces;

/// <summary>
/// Abstraction for escalating fraud alerts (e.g., freeze wallet, notify admin).
/// </summary>
public interface IFraudEscalationService
{
    /// <summary>
    /// Escalates a fraud alert for the given wallet.
    /// </summary>
    /// <param name="walletId">The wallet involved.</param>
    /// <param name="ruleName">The fraud rule triggered.</param>
    /// <param name="description">Details of the event.</param>
    Task EscalateAsync(Guid walletId, string ruleName, string description, CancellationToken cancellationToken);
}