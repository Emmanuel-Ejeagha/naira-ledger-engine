namespace NairaLedger.Tests.Infrastructure;

public class TransactionQueryServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task GetTransactions_WithCursorPagination_ShouldReturnCorrectPage()
    {
        var walletRepo = ServiceProvider.GetRequiredService<IWalletRepository>();
        var transactionRepo = ServiceProvider.GetRequiredService<ITransactionRepository>();
        var unitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
        var queryService = ServiceProvider.GetRequiredService<ITransactionQueryService>();

        var wallet = new Wallet(new UserId(Guid.NewGuid()));
        await walletRepo.AddAsync(wallet);
        await unitOfWork.SaveChangesAsync();

        // Create 5 transactions for this wallet
        for (int i = 0; i < 5; i++)
        {
            var tx = new Transaction(TransactionReference.Generate(),
                TransactionType.Funding,
                new List<LedgerEntry>
                {
                    new(Guid.NewGuid(), 10, LedgerEntryDirection.Debit, "bank"),
                    new(wallet.Id, 10, LedgerEntryDirection.Credit, "fund")
                }, null);
            await transactionRepo.AddAsync(tx);
            // Slight delay to ensure different CreatedAt
            await Task.Delay(10);
        }
        await unitOfWork.SaveChangesAsync();

        // First page with pageSize=3
        var page1 = await queryService.GetTransactionsAsync(wallet.Id, null, 3, CancellationToken.None);
        page1.Items.Should().HaveCount(3);
        page1.HasMore.Should().BeTrue();
        page1.NextCursor.Should().NotBeNullOrEmpty();

        // Next page
        var page2 = await queryService.GetTransactionsAsync(wallet.Id, page1.NextCursor, 3, CancellationToken.None);
        page2.Items.Should().HaveCount(2); // remaining
        page2.HasMore.Should().BeFalse();
    }
}