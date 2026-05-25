namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Read‑only transaction query service.
/// </summary>
public interface ITransactionQueryService
{
    /// <summary>
    /// Returns a cursor‑paginated list of transactions affecting the given wallet.
    /// </summary>
    Task<PagedResponse<TransactionDto>> GetTransactionsAsync(Guid walletId, string? cursor, int pageSize, CancellationToken cancellationToken);
}