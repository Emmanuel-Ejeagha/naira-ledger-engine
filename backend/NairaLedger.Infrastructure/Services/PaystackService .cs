using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NairaLedger.Application.Commands.FundWallet;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.ValueObjects;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NairaLedger.Infrastructure.Services;

public class PaystackService : IPaystackService
{
    private readonly string _secretKey;
    private readonly IUserWalletResolver _walletResolver;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaystackService> _logger;

    public PaystackService(
        IOptions<PaystackSettings> settings,
        IUserWalletResolver walletResolver,
        HttpClient httpClient,
        ILogger<PaystackService> logger)
    {
        _secretKey = settings.Value.SecretKey;
        _walletResolver = walletResolver;
        _httpClient = httpClient;
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
        var amount = data.GetProperty("amount").GetInt32() / 100m;
        var reference = data.GetProperty("reference").GetString()!;
        var customerEmail = data.GetProperty("customer").GetProperty("email").GetString()!;

        var wallet = await _walletResolver.GetWalletByEmailAsync(customerEmail, cancellationToken);

        if (wallet is null)
        {
            var metadata = data.TryGetProperty("metadata", out var meta) ? meta : default;
            var walletIdStr = metadata.TryGetProperty("wallet_id", out var wid) ? wid.GetString() : null;
            if (!string.IsNullOrEmpty(walletIdStr) && Guid.TryParse(walletIdStr, out var walletId))
            {
                _logger.LogInformation("Using metadata wallet_id {WalletId} for funding", walletId);
                return new FundWalletCommand(walletId, amount, new IdempotencyKey(reference));
            }

            _logger.LogError("Paystack event {Reference}: no wallet found", reference);
            return null;
        }

        return new FundWalletCommand(wallet.Id, amount, new IdempotencyKey(reference));
    }

    public async Task<PaystackVerificationResult?> VerifyTransactionAsync(string reference, CancellationToken cancellationToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _secretKey);

            var response = await _httpClient.GetAsync($"transaction/verify/{reference}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Paystack verify failed for {Reference}. Status: {StatusCode}", reference, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadFromJsonAsync<PaystackVerificationResponse>(cancellationToken: cancellationToken);
            if (content?.Data is null || !content.Status)
                return null;

            return new PaystackVerificationResult(
                content.Data.Status,
                content.Data.Amount / 100m,
                content.Data.Currency ?? "NGN"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Paystack transaction {Reference}", reference);
            return null;
        }
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

    private record PaystackVerificationResponse(bool Status, PaystackVerificationData? Data);
    private record PaystackVerificationData(string Status, int Amount, string? Currency);
}