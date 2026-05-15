using MediatR;
using NairaWallet.Application.DTOs;

namespace NairaWallet.Application.Queries.GetTransactionHistory;

/// <summary>
/// Retrieves a paginated list of transactions for a wallet, sorted by creation time descending.
/// </summary>
public record GetTransactionHistoryQuery(
    Guid WalletId,
    string? Cursor,
    int PageSize = 20) : IRequest<PagedResponse<TransactionDto>>;