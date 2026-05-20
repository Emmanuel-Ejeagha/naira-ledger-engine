using Microsoft.Extensions.Logging;
using NairaLedger.Application.Commands.FundWallet;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NairaLedger.Infrastructure.Services;

/// <summary>
/// Verifies Paystack webhook signatures and resolves the target wallet from the customer email.
/// Metadata wallet_id is used as fallback if present.
/// </summary>
public class PaystackService : IPaystackService
{
    private readonly string _secretKey;
    private readonly IUserWalletResolver _walletResolver;
    private readonly ILogger<PaystackService> _logger;

    public PaystackService(string secretKey, IUserWalletResolver walletResolver, ILogger<PaystackService> logger)
    {
        _secretKey = secretKey;
        _walletResolver = walletResolver;
        _logger = logger;
    }

    public async Task<FundWalletCommand?> ProcessWebhookAsync(string payload, string signature, CancellationToken cancellationToken)
    {
        if (!VerifySignature(payload, signature))
        {
            _logger.LogWarning("Paystack webhook signature verification failed");
            return null;
        }

        var json = JsonDocument.Parse(payload);
        var root = json.RootElement;
        var eventType = root.GetProperty("event").GetString();

        if (eventType != "charge.success")
        {
            _logger.LogInformation("Ignoring Paystack event type: {EventType}", eventType);
            return null;
        }

        var data = root.GetProperty("data");
        var amount = data.GetProperty("amount").GetInt32() / 100m; // kobo to Naira
        var reference = data.GetProperty("reference").GetString()!;
        var status = data.GetProperty("status").GetString();
        var customerEmail = data.GetProperty("customer").GetProperty("email").GetString()!;

        // Primary resolution: find wallet by customer email
        var wallet = await _walletResolver.GetWalletByEmailAsync(customerEmail, cancellationToken);

        // Fallback to metadata wallet_id if explicitly provided
        if (wallet is null)
        {
            var metadata = data.TryGetProperty("metadata", out var meta) ? meta : default;
            var walletIdStr = metadata.TryGetProperty("wallet_id", out var wid) ? wid.GetString() : null;

            if (!string.IsNullOrEmpty(walletIdStr) && Guid.TryParse(walletIdStr, out var walletId))
            {
                _logger.LogInformation("Using metadata wallet_id {WalletId} for funding", walletId);
                return new FundWalletCommand(walletId, amount, new IdempotencyKey(reference));
            }

            _logger.LogError("Paystack event {Reference}: no wallet found for email {Email} or metadata", reference, customerEmail);
            return null;
        }

        return new FundWalletCommand(wallet.Id, amount, new IdempotencyKey(reference));
    }

    private bool VerifySignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_secretKey))
            return false;

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return computed == signature;
    }
}