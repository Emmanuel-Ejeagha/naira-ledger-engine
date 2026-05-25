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

    /// <summary>
    /// Verifies a transaction reference with Paystack and returns the status/details.
    /// </summary>
    Task<PaystackVerificationResult?> VerifyTransactionAsync(string reference, CancellationToken cancellationToken);
}

public record PaystackVerificationResult(string Status, decimal Amount, string Currency);