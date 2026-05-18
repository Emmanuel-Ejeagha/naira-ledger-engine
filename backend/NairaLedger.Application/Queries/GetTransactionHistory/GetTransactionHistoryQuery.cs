using MediatR;
using NairaLedger.Application.DTOs;

namespace NairaLedger.Application.Queries.GetTransactionHistory;

/// <summary>
/// Retrieves a paginated list of transactions for a wallet, sorted by creation time descending.
/// </summary>
public record GetTransactionHistoryQuery(
    Guid WalletId,
    string? Cursor,
    int PageSize = 20) : IRequest<PagedResponse<TransactionDto>>;