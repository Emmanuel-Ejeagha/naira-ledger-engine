using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;
using NairaLedger.Tests.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace NairaLedger.Tests.Infrastructure;

public class PaystackServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task ProcessWebhook_ValidSignatureAndEmailMatch_ShouldReturnCommand()
    {
        // Arrange: create user & wallet for email matching
        var userService = ServiceProvider.GetRequiredService<IUserService>();
        var walletRepo = ServiceProvider.GetRequiredService<IWalletRepository>();
        var unitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();

        var userResult = await userService.CreateUserAsync("customer@example.com", "Customer", "Customer1!", CancellationToken.None);
        var wallet = new Wallet(new UserId(userResult.UserId));
        await walletRepo.AddAsync(wallet);
        await unitOfWork.SaveChangesAsync();

        var paystackService = ServiceProvider.GetRequiredService<IPaystackService>();

        var payload = @"{
            ""event"": ""charge.success"",
            ""data"": {
                ""amount"": 500000,
                ""reference"": ""ref-123"",
                ""status"": ""success"",
                ""customer"": { ""email"": ""customer@example.com"" },
                ""metadata"": {}
            }
        }";
        var signature = ComputeHmac(payload, "test_secret");

        var command = await paystackService.ProcessWebhookAsync(payload, signature, CancellationToken.None);

        command.Should().NotBeNull();
        command!.WalletId.Should().Be(wallet.Id);
        command.Amount.Should().Be(5000);
        command.IdempotencyKey.Value.Should().Be("ref-123");
    }

    [Fact]
    public async Task ProcessWebhook_InvalidSignature_ShouldReturnNull()
    {
        var paystackService = ServiceProvider.GetRequiredService<IPaystackService>();
        var payload = "{}";
        var signature = "invalid";

        var command = await paystackService.ProcessWebhookAsync(payload, signature, CancellationToken.None);
        command.Should().BeNull();
    }

    private static string ComputeHmac(string payload, string secret)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}