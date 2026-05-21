using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Application.Commands.FundWallet;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests.Application;

/// <summary>
/// Tests for FundWalletCommandHandler covering success, inactive wallet, and missing wallet scenarios.
/// </summary>
public class FundWalletCommandTests
{
    private readonly Mock<IWalletRepository> _walletRepoMock = new();
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<FundWalletCommandHandler>> _loggerMock = new();
    private readonly FundWalletCommandHandler _handler;

    public FundWalletCommandTests()
    {
        _handler = new FundWalletCommandHandler(
            _walletRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidActiveWallet_ShouldSucceedAndPersistTransaction()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var wallet = CreateWallet(walletId, isActive: true);

        _walletRepoMock.Setup(r => r.GetByIdAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        var command = new FundWalletCommand(walletId, 5000, new IdempotencyKey(Guid.NewGuid().ToString()));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.TransactionId.Should().NotBeEmpty();
        response.Message.Should().Contain("funded");
        _transactionRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InactiveWallet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var inactiveWallet = CreateWallet(walletId, isActive: false);

        _walletRepoMock.Setup(r => r.GetByIdAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveWallet);

        var command = new FundWalletCommand(walletId, 100, new IdempotencyKey(Guid.NewGuid().ToString()));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*inactive*");
    }

    [Fact]
    public async Task Handle_WalletNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var missingWalletId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByIdAsync(missingWalletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var command = new FundWalletCommand(missingWalletId, 50, new IdempotencyKey(Guid.NewGuid().ToString()));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    /// <summary>
    /// Creates a Wallet aggregate with a specific ID and active status.
    /// Uses reflection to set the private setters because the domain model
    /// intentionally forbids external mutation of these properties.
    /// </summary>
    private static Wallet CreateWallet(Guid id, bool isActive)
    {
        var wallet = new Wallet(new UserId(Guid.NewGuid()));

        typeof(Wallet).GetProperty(nameof(Wallet.Id))!
            .SetValue(wallet, id);

        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!
            .SetValue(wallet, isActive);

        return wallet;
    }
}