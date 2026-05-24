namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Abstraction for payment providers (Paystack).
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Initiates a payment and returns an authorization URL and reference.
    /// </summary>
    Task<(string AuthorizationUrl, string Reference)> InitiatePaymentAsync(decimal amount, string email, string callbackUrl, Guid walletId, CancellationToken cancellationToken);
}