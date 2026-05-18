using MediatR;

namespace NairaLedger.Application.Queries.GetWalletBalance;

/// <summary>
/// Retrieves the current balance of a wallet.
/// </summary>
public record GetWalletBalanceQuery(Guid WalletId) : IRequest<WalletBalanceDto>;

/// <summary>
/// Wallet balance data transfer object.
/// </summary>
public record WalletBalanceDto(Guid WalletId, decimal Balance, string Currency);