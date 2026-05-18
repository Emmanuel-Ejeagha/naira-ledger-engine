using MediatR;
using NairaLedger.Application.Commands;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Application.Commands.FundWallet;

/// <summary>
/// Credits a wallet (e.g., after successful Paystack webhook).
/// </summary>
/// <param name="WalletId">Target wallet.</param>
/// <param name="Amount">NGN amount to credit.</param>
/// <param name="IdempotencyKey">Unique key to prevent double‑funding.</param>
public record FundWalletCommand(Guid WalletId, decimal Amount, IdempotencyKey IdempotencyKey)
    : IRequest<FundWalletResponse>, IIdempotentCommand;

/// <summary>
/// Result of a funding operation.
/// </summary>
public record FundWalletResponse(Guid TransactionId, decimal NewBalance, string Message);