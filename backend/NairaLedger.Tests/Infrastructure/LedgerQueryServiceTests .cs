using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Entities;
using NairaLedger.Domain.Enums;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests.Infrastructure;

public class LedgerQueryServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task GetBalance_AfterMultipleTransactions_ShouldBeCorrect()
    {
        var walletRepo = ServiceProvider.GetRequiredService<IWalletRepository>();
        var transactionRepo = ServiceProvider.GetRequiredService<ITransactionRepository>();
        var unitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();
        var ledgerQuery = ServiceProvider.GetRequiredService<ILedgerQueryService>();

        var wallet = new Wallet(new UserId(Guid.NewGuid()));
        await walletRepo.AddAsync(wallet);
        await unitOfWork.SaveChangesAsync();

        // Credit 1000
        var tx1 = new Transaction(TransactionReference.Generate(),
            TransactionType.Funding,
            new List<LedgerEntry>
            {
                new(Guid.NewGuid(), 1000, LedgerEntryDirection.Debit, "bank"),
                new(wallet.Id, 1000, LedgerEntryDirection.Credit, "fund")
            }, null);
        await transactionRepo.AddAsync(tx1);

        // Debit 300
        var tx2 = new Transaction(TransactionReference.Generate(),
            TransactionType.Transfer,
            new List<LedgerEntry>
            {
                new(wallet.Id, 300, LedgerEntryDirection.Debit, "send"),
                new(Guid.NewGuid(), 300, LedgerEntryDirection.Credit, "receive")
            }, null);
        await transactionRepo.AddAsync(tx2);

        await unitOfWork.SaveChangesAsync(); // <-- ensure save before query

        var balance = await ledgerQuery.GetBalanceAsync(wallet.Id);
        balance.Should().Be(700);
    }
}