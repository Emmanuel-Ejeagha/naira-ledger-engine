using MediatR;

namespace NairaLedger.Application.Commands.ReverseTransaction;

/// <summary>
/// Reverses a completed transaction within the 30‑minute window.
/// </summary>
/// <param name="TransactionId">ID of the transaction to reverse.</param>
/// <param name="InitiatedByUserId">The user requesting the reversal.</param>
public record ReverseTransactionCommand(Guid TransactionId, Guid? InitiatedByUserId) : IRequest<ReverseTransactionResponse>;

/// <summary>
/// Result of a reversal.
/// </summary>
public record ReverseTransactionResponse(Guid ReversalTransactionId, string Message);