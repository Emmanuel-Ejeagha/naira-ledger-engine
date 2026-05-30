namespace NairaLedger.Tests.Infrastructure;

public class WalletRepositoryTests : IntegrationTestBase
{
    private IWalletRepository WalletRepo => ServiceProvider.GetRequiredService<IWalletRepository>();
    private IUnitOfWork UnitOfWork => ServiceProvider.GetRequiredService<IUnitOfWork>();

    [Fact]
    public async Task AddWallet_ThenGetById_ShouldReturnSameWallet()
    {
        var userId = new UserId(Guid.NewGuid());
        var wallet = new Wallet(userId, new WalletTag("Savings"));

        await WalletRepo.AddAsync(wallet);
        await UnitOfWork.SaveChangesAsync();

        var retrieved = await WalletRepo.GetByIdAsync(wallet.Id);
        retrieved.Should().NotBeNull();
        retrieved!.UserId.Should().Be(userId);
        retrieved.Tag!.Value.Should().Be("Savings");
    }

    [Fact]
    public async Task GetByUserId_WhenExists_ShouldReturnWallet()
    {
        var userId = new UserId(Guid.NewGuid());
        var wallet = new Wallet(userId);

        await WalletRepo.AddAsync(wallet);
        await UnitOfWork.SaveChangesAsync();

        var result = await WalletRepo.GetByUserIdAsync(userId);
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetOwnerInfoAsync_WhenWalletExists_ShouldReturnUserEmailAndName()
    {
        var userManager = ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<NairaLedger.Infrastructure.Identity.AppUser>>();
        var user = new NairaLedger.Infrastructure.Identity.AppUser
        {
            UserName = "owner@example.com",
            Email = "owner@example.com",
            FullName = "Owner Full"
        };
        await userManager.CreateAsync(user, "OwnerPass1!");

        var wallet = new Wallet(new UserId(user.Id));
        await WalletRepo.AddAsync(wallet);
        await UnitOfWork.SaveChangesAsync();

        var info = await WalletRepo.GetOwnerInfoAsync(wallet.Id);
        info.Should().NotBeNull();
        info!.Email.Should().Be("owner@example.com");
        info.FullName.Should().Be("Owner Full");
    }
}