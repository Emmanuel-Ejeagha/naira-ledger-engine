namespace NairaLedger.Tests.Infrastructure;

public class FraudDetectionServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task EscalateHighVelocity_ShouldFreezeWallet()
    {
        var walletRepo = ServiceProvider.GetRequiredService<IWalletRepository>();
        var unitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
        var fraudService = ServiceProvider.GetRequiredService<IFraudEscalationService>();

        var wallet = new Wallet(new UserId(Guid.NewGuid()));
        await walletRepo.AddAsync(wallet);
        await unitOfWork.SaveChangesAsync();

        await fraudService.EscalateAsync(wallet.Id, "HighVelocity", "Test", CancellationToken.None);

        var frozen = await walletRepo.GetByIdAsync(wallet.Id);
        frozen.Should().NotBeNull();
        frozen!.IsActive.Should().BeFalse();
    }
}