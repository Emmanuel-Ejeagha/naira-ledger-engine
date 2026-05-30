namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Provides read‑only balance and ledger queries without exposing infrastructure.
/// </summary>
public interface ILedgerQueryService
{
    /// <summary>
    /// Computes the current balance of a wallet by aggregating ledger entries.
    /// </summary>
    Task<decimal> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);
}