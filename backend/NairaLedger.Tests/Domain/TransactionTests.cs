namespace NairaLedger.Tests.Domain;

public class TransactionTests
{
    private readonly Guid _walletA = Guid.NewGuid();
    private readonly Guid _walletB = Guid.NewGuid();
    private readonly Guid _bank = Guid.NewGuid();

    [Fact]
    public void Create_WithBalanceEntries_ShouldSuccedAndRaiseEvent()
    {
        // Arrange
        var entries = new List<LedgerEntry>
        {
            new(_walletA, 1000m, LedgerEntryDirection.Debit, "Transfer to B"),
            new(_walletB, 1000m, LedgerEntryDirection.Credit, "Transfer from A")
        };


        // Act
        var tx = new Transaction(
            TransactionReference.Generate(),
            TransactionType.Transfer,
            entries,
            null);

        // Assert
        tx.Status.Should().Be(TransactionStatus.Completed);
        tx.Entries.Should().HaveCount(2);
        tx.DomainEvents.Should().ContainSingle(e => e is TransferCompletedEvent);
    }

    [Fact]
    public void Create_WithUnbalancedEEntries_ThrowsInvalidOperationException()
    {
        // Arrange
        var entries = new List<LedgerEntry>
        {
            new(_walletA, 1000m, LedgerEntryDirection.Debit, "Transfer to B"),
            new(_walletB, 900m, LedgerEntryDirection.Credit, "Transfer from A")
        };
        // Act
        Action act = () => new Transaction(
            TransactionReference.Generate(),
            TransactionType.Transfer,
            entries,
            null);
        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*does not balance*");
    }

    [Fact]
    public void Create_WithLessThanTwoEntries_ThrowsArgumentException() 
    {
        var entries = new List<LedgerEntry>
        {
            new(_walletA, 1000m, LedgerEntryDirection.Debit, "Transfer to B")
        };
        // Act
        Action act = () => new Transaction(
            TransactionReference.Generate(),
            TransactionType.Transfer,
            entries,
            null);
        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*at least two*");
    }

    [Fact]
    public void Funding_ShouldRaiseWalletFundedEvent()
    {
        // Arrange
        var entries = new List<LedgerEntry>
        {
            new(_bank, 5000m, LedgerEntryDirection.Debit, "Bank funding"),
            new(_walletA, 5000m, LedgerEntryDirection.Credit, "Wallet top-up")
        };

        // Act
        var tx = new Transaction(
            TransactionReference.Generate(),
            TransactionType.Funding,
            entries,
            null);

        // Assert
        tx.DomainEvents.Should().ContainSingle(e => e is WalletFundedEvent);
        var fundingEvent = tx.DomainEvents.OfType<WalletFundedEvent>().Single();
        fundingEvent.WalletId.Should().Be(_walletA);
        fundingEvent.Amount.Should().Be(5000m);
    }

    [Fact]
    public void CreateReversal_Within30Minutes_ShouldProduceBalanceReversal()
    {
        var original = CreateCompletedTransfer();
        var reversal = Transaction.CreateReversal(original, null);

        reversal.Should().NotBeNull();
        reversal.Type.Should().Be(TransactionType.Reversal);
        reversal.Status.Should().Be(TransactionStatus.Completed);
        reversal.Entries.Should().HaveCount(2);

        var debitEntry = reversal.Entries.Single(e => e.Direction == LedgerEntryDirection.Debit);
        var creditEntry = reversal.Entries.Single(e => e.Direction == LedgerEntryDirection.Credit);

        debitEntry.WalletId.Should().Be(_walletB);
        creditEntry.WalletId.Should().Be(_walletA);

        reversal.DomainEvents.Should().ContainSingle(e => e is TransactionReversedEvent);
    }

    [Fact]
    public void CreateReversal_After30Minuties_ShouldThrow()
    {
        var staleTransaction = CreateStaleTransaction(TimeSpan.FromMinutes(31));

        Action act = () => Transaction.CreateReversal(staleTransaction, null);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*30 minutes*");
    }

    [Fact]
    public void CreateReversal_OnAlreadyReversedTransaction_ShouldThrow()
    {
        var original = CreateCompletedTransfer();
        var reversal = Transaction.CreateReversal(original, null);

        Action act = () => Transaction.CreateReversal(reversal, null);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be reversed again*");
    }

    private Transaction CreateStaleTransaction(TimeSpan ago)
    {
        var entries = new List<LedgerEntry>
        {
            new(_walletA, 1000m, LedgerEntryDirection.Debit, "old"),
            new(_walletB, 1000m, LedgerEntryDirection.Credit, "old")
        };

        var tx = new Transaction(
            TransactionReference.Generate(),
            TransactionType.Transfer,
            entries,
            null);
        typeof(Transaction).GetProperty(nameof(Transaction.CreatedAt))!
            .SetValue(tx, DateTime.UtcNow - ago);
        return tx;
    }

    private Transaction CreateCompletedTransfer()
    {
        var entries = new List<LedgerEntry>
        {
            new(_walletA, 1000m, LedgerEntryDirection.Debit, "P2P send"),
            new(_walletB, 1000m, LedgerEntryDirection.Credit, "P2P receive")
        };
        return new Transaction(
            TransactionReference.Generate(),
            TransactionType.Transfer,
            entries,
            null);
    }
}
