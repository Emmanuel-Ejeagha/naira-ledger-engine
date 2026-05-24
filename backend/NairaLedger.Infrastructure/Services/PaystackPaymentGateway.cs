using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NairaLedger.Application.Interfaces;

namespace NairaLedger.Infrastructure.Services;

public class PaystackPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<PaystackPaymentGateway> _logger;

    public PaystackPaymentGateway(
        HttpClient httpClient,
        IOptions<PaystackSettings> settings,
        ILogger<PaystackPaymentGateway> logger)
    {
        _httpClient = httpClient;
        _secretKey = settings.Value.SecretKey;
        _logger = logger;
    }

    public async Task<(string AuthorizationUrl, string Reference)> InitiatePaymentAsync(decimal amount, string email, string callbackUrl, Guid walletId, CancellationToken cancellationToken)
    {
        var koboAmount = (int)(amount * 100);
        var reference = $"NW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..24];
        var payload = new
        {
            amount = koboAmount,
            email,
            reference,
            callback_url = callbackUrl,
            metadata = new { wallet_id = walletId.ToString() }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {_secretKey}");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var data = result.GetProperty("data");
        var authorizationUrl = data.GetProperty("authorization_url").GetString()!;
        return (authorizationUrl, reference);
    }
}