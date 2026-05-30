namespace NairaLedger.Application.Queries.GetTransactionHistory;

/// <summary>
/// Handles transaction history queries via ITransactionQueryService.
/// </summary>
public class GetTransactionHistoryHandler : IRequestHandler<GetTransactionHistoryQuery, PagedResponse<TransactionDto>>
{
    private readonly ITransactionQueryService _transactionQueryService;

    public GetTransactionHistoryHandler(ITransactionQueryService transactionQueryService)
    {
        _transactionQueryService = transactionQueryService;
    }

    public async Task<PagedResponse<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _transactionQueryService.GetTransactionsAsync(
            request.WalletId, request.Cursor, request.PageSize, cancellationToken);
    }
}