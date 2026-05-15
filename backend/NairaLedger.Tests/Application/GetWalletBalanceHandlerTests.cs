using FluentAssertions;
using Moq;
using NairaWallet.Application.Interfaces;
using NairaWallet.Application.Queries.GetWalletBalance;

namespace NairaWallet.Tests.Application;

/// <summary>
/// Tests for the wallet balance query handler.
/// </summary>
public class GetWalletBalanceHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnBalanceFromLedgerService()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var expectedBalance = 1500.75m;

        var mockLedger = new Mock<ILedgerQueryService>();
        mockLedger.Setup(l => l.GetBalanceAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBalance);

        var handler = new GetWalletBalanceHandler(mockLedger.Object);
        var query = new GetWalletBalanceQuery(walletId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.WalletId.Should().Be(walletId);
        result.Balance.Should().Be(expectedBalance);
        result.Currency.Should().Be("NGN");
    }
}