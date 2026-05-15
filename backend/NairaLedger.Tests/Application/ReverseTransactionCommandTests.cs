using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Entities;
using NairaLedger.Domain.Enums;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;
using NairaWallet.Application.Commands.ReverseTransaction;
using NairaWallet.Application.Interfaces;
namespace NairaWallet.Tests.Application;

/// <summary>
/// Tests for ReverseTransactionCommandHandler covering successful reversal,
/// expired window, and missing transaction.
/// </summary>
public class ReverseTransactionCommandTests
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<ReverseTransactionCommandHandler>> _loggerMock = new();
    private readonly ReverseTransactionCommandHandler _handler;

    public ReverseTransactionCommandTests()
    {
        _handler = new ReverseTransactionCommandHandler(
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidTransactionWithinWindow_ShouldSucceedAndPersistReversal()
    {
        // Arrange
        var original = CreateValidTransfer();
        _transactionRepoMock.Setup(r => r.GetByIdWithEntriesAsync(original.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(original);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        var command = new ReverseTransactionCommand(original.Id, Guid.NewGuid());

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.ReversalTransactionId.Should().NotBeEmpty();
        response.Message.Should().Contain("reversed");
        _transactionRepoMock.Verify(
            r => r.AddAsync(It.Is<Transaction>(tx => tx.Type == TransactionType.Reversal), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_TransactionNotFound_ShouldThrow()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _transactionRepoMock.Setup(r => r.GetByIdWithEntriesAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);

        var command = new ReverseTransactionCommand(missingId, null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_TransactionOutsideReversalWindow_ShouldThrow()
    {
        // Arrange
        var stale = CreateStaleTransfer(TimeSpan.FromMinutes(31));
        _transactionRepoMock.Setup(r => r.GetByIdWithEntriesAsync(stale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stale);

        var command = new ReverseTransactionCommand(stale.Id, null);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*30 minutes*");
    }

    // ----- helpers -----

    private static Transaction CreateValidTransfer()
    {
        var entries = new List<LedgerEntry>
        {
            new(Guid.NewGuid(), 1000, LedgerEntryDirection.Debit, "Send"),
            new(Guid.NewGuid(), 1000, LedgerEntryDirection.Credit, "Receive")
        };
        var tx = new Transaction(TransactionReference.Generate(), TransactionType.Transfer, entries, null);
        // Override CreatedAt to be very recent (within window) – no change needed, it's fresh
        return tx;
    }

    private static Transaction CreateStaleTransfer(TimeSpan age)
    {
        var tx = CreateValidTransfer();
        typeof(Transaction).GetProperty(nameof(Transaction.CreatedAt))!
            .SetValue(tx, DateTime.UtcNow - age);
        return tx;
    }
}