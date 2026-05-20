using NairaLedger.Application.Commands.FundWallet;

namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Abstraction for verifying and processing Paystack webhook events.
/// </summary>
public interface IPaystackService
{
    /// <summary>
    /// Verifies the webhook signature and returns a FundWalletCommand if the event is valid.
    /// Returns null if the event should be ignored (e.g., duplicate or unsupported type).
    /// </summary>
    /// <param name="payload">Raw request body.</param>
    /// <param name="signature">The x-paystack-signature header.</param>
    Task<FundWalletCommand?> ProcessWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default);
}