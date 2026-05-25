namespace NairaWallet.Tests.Infrastructure;

public class TransactionRepositoryTests : IntegrationTestBase
{
    private IWalletRepository WalletRepo => ServiceProvider.GetRequiredService<IWalletRepository>();
    private ITransactionRepository TransactionRepo => ServiceProvider.GetRequiredService<ITransactionRepository>();
    private IUnitOfWork UnitOfWork => ServiceProvider.GetRequiredService<IUnitOfWork>();
    private ILedgerQueryService LedgerQuery => ServiceProvider.GetRequiredService<ILedgerQueryService>();

    [Fact]
    public async Task AddTransaction_WithBalancedEntries_ShouldPersistAndReflectInBalance()
    {
        var walletA = new Wallet(new UserId(Guid.NewGuid()));
        var walletB = new Wallet(new UserId(Guid.NewGuid()));
        await WalletRepo.AddAsync(walletA);
        await WalletRepo.AddAsync(walletB);
        await UnitOfWork.SaveChangesAsync();

        var entries = new List<LedgerEntry>
        {
            new(walletA.Id, 500, LedgerEntryDirection.Debit, "Send"),
            new(walletB.Id, 500, LedgerEntryDirection.Credit, "Receive")
        };
        var tx = new Transaction(TransactionReference.Generate(), TransactionType.Transfer, entries, null);

        await TransactionRepo.AddAsync(tx);
        await UnitOfWork.SaveChangesAsync();

        var retrieved = await TransactionRepo.GetByIdWithEntriesAsync(tx.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Entries.Should().HaveCount(2);
        retrieved.Status.Should().Be(TransactionStatus.Completed);

        var balanceA = await LedgerQuery.GetBalanceAsync(walletA.Id);
        var balanceB = await LedgerQuery.GetBalanceAsync(walletB.Id);
        balanceA.Should().Be(-500);
        balanceB.Should().Be(500);
    }

    [Fact]
    public async Task AddTransaction_DuplicateReference_ShouldThrow()
    {
        var wallet = new Wallet(new UserId(Guid.NewGuid()));
        await WalletRepo.AddAsync(wallet);
        await UnitOfWork.SaveChangesAsync();

        var ref1 = TransactionReference.Generate();
        var tx1 = new Transaction(ref1, TransactionType.Funding,
            new List<LedgerEntry>
            {
                new(Guid.NewGuid(), 100, LedgerEntryDirection.Debit, "out"),
                new(wallet.Id, 100, LedgerEntryDirection.Credit, "in")
            }, null);
        await TransactionRepo.AddAsync(tx1);
        await UnitOfWork.SaveChangesAsync();

        var tx2 = new Transaction(ref1, TransactionType.Transfer,
            new List<LedgerEntry>
            {
                new(wallet.Id, 50, LedgerEntryDirection.Debit, "out"),
                new(Guid.NewGuid(), 50, LedgerEntryDirection.Credit, "in")
            }, null);

        await TransactionRepo.AddAsync(tx2);
        Func<Task> act = () => UnitOfWork.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>(); // unique constraint
    }
}