namespace NairaLedger.Application.Commands.TransferFunds;

/// <summary>
/// Initiates a peer‑to‑peer transfer between two wallets.
/// </summary>
/// <param name="FromWalletId">Sender wallet.</param>
/// <param name="ToWalletId">Recipient wallet.</param>
/// <param name="Amount">NGN amount to transfer.</param>
/// <param name="IdempotencyKey">Unique key to prevent duplicate transfers.</param>
public record TransferCommand(
    Guid FromWalletId, 
    Guid ToWalletId, 
    decimal Amount, 
    string IdempotencyKey)
    : IRequest<TransferResponse>, IIdempotentCommand;

/// <summary>
/// Result of a transfer operation.
/// </summary>
public record TransferResponse(Guid TransactionId, string Message);