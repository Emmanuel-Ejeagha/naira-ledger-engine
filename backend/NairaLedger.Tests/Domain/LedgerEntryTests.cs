using FluentAssertions;
using NairaLedger.Domain.Entities;
using NairaLedger.Domain.Enums;

namespace NairaLedger.Tests.Domain;

public class LedgerEntryTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldSucced()
    {
        // Arrange
        var walletId = Guid.NewGuid();

        // Act
        var entry = new LedgerEntry(walletId, 1345m, LedgerEntryDirection.Credit, "Funding");

        // Assert
        entry.WalletId.Should().Be(walletId);
        entry.Amount.Should().Be(1345m);
        entry.Direction.Should().Be(LedgerEntryDirection.Credit);
        entry.Description.Should().Be("Funding");
        entry.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveAmount_ThrowsArgumentException(decimal amount)
    {
        Action act = () => new LedgerEntry(Guid.NewGuid(), amount, LedgerEntryDirection.Debit, "test");
        act.Should().Throw<ArgumentException>().WithMessage("*positive*");
    }

    [Fact]
    public void Constructor_WithEmptyWalletId_ThrowsArgumentException()
    {
        Action act = () => new LedgerEntry(Guid.Empty, 100m, LedgerEntryDirection.Debit, "test");
        act.Should().Throw<ArgumentException>().WithMessage("*Wallet ID*");
    }
}
