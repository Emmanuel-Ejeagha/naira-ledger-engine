using MediatR;
using NairaLedger.Application.Interfaces;

namespace NairaLedger.Application.Queries.GetWalletBalance;

/// <summary>
/// Handles balance queries by calling the ledger query service.
/// </summary>
public class GetWalletBalanceHandler : IRequestHandler<GetWalletBalanceQuery, WalletBalanceDto>
{
    private readonly ILedgerQueryService _ledgerQueryService;

    public GetWalletBalanceHandler(ILedgerQueryService ledgerQueryService)
    {
        _ledgerQueryService = ledgerQueryService;
    }

    public async Task<WalletBalanceDto> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        var balance = await _ledgerQueryService.GetBalanceAsync(request.WalletId, cancellationToken);
        return new WalletBalanceDto(request.WalletId, balance, "NGN");
    }
}