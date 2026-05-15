using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;
using NairaWallet.Application.Commands.TransferFunds;
using NairaWallet.Application.Interfaces;

namespace NairaWallet.Tests.Application;

/// <summary>
/// Tests transfer command handler including balance check.
/// </summary>
public class TransferCommandTests
{
    private readonly Mock<IWalletRepository> _walletRepoMock = new();
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
    private readonly Mock<ILedgerQueryService> _ledgerQueryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<TransferCommandHandler>> _loggerMock = new();
    private readonly TransferCommandHandler _handler;

    public TransferCommandTests()
    {
        _handler = new TransferCommandHandler(
            _walletRepoMock.Object,
            _transactionRepoMock.Object,
            _ledgerQueryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithSufficientFunds_ShouldSucceed()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();

        var fromWallet = new Wallet(new UserId(Guid.NewGuid()));
        typeof(Wallet).GetProperty(nameof(Wallet.Id))!.SetValue(fromWallet, fromId);
        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!.SetValue(fromWallet, true);

        var toWallet = new Wallet(new UserId(Guid.NewGuid()));
        typeof(Wallet).GetProperty(nameof(Wallet.Id))!.SetValue(toWallet, toId);
        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!.SetValue(toWallet, true);

        _walletRepoMock.Setup(r => r.GetByIdAsync(fromId, It.IsAny<CancellationToken>())).ReturnsAsync(fromWallet);
        _walletRepoMock.Setup(r => r.GetByIdAsync(toId, It.IsAny<CancellationToken>())).ReturnsAsync(toWallet);
        _ledgerQueryMock.Setup(q => q.GetBalanceAsync(fromId, It.IsAny<CancellationToken>())).ReturnsAsync(2000);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        var command = new TransferCommand(fromId, toId, 500, new IdempotencyKey(Guid.NewGuid().ToString()));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.TransactionId.Should().NotBeEmpty();
        response.Message.Should().Contain("completed");
        _transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ShouldThrow()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();

        var fromWallet = new Wallet(new UserId(Guid.NewGuid()));
        typeof(Wallet).GetProperty(nameof(Wallet.Id))!.SetValue(fromWallet, fromId);
        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!.SetValue(fromWallet, true);

        var toWallet = new Wallet(new UserId(Guid.NewGuid()));
        typeof(Wallet).GetProperty(nameof(Wallet.Id))!.SetValue(toWallet, toId);
        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!.SetValue(toWallet, true);

        _walletRepoMock.Setup(r => r.GetByIdAsync(fromId, It.IsAny<CancellationToken>())).ReturnsAsync(fromWallet);
        _walletRepoMock.Setup(r => r.GetByIdAsync(toId, It.IsAny<CancellationToken>())).ReturnsAsync(toWallet);
        _ledgerQueryMock.Setup(q => q.GetBalanceAsync(fromId, It.IsAny<CancellationToken>())).ReturnsAsync(200);

        var command = new TransferCommand(fromId, toId, 500, new IdempotencyKey(Guid.NewGuid().ToString()));

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Insufficient funds*");
    }
}