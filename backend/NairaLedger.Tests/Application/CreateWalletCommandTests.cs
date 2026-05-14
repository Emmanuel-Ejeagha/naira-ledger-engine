using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Application.Commands.CreateWallet;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;
using NairaWallet.Application.Interfaces;

namespace NairaLedger.Tests.Application;

public class CreateWalletCommandTests
{
    private readonly Mock<IWalletRepository> _walletRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<CreateWalletCommandHandler>> _loggerMock = new();
    private readonly CreateWalletCommandHandler _handler;

    public CreateWalletCommandTests()
    {
        _handler = new CreateWalletCommandHandler(
            _walletRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNotExistingWallet_ShouldCreateNewWallet()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var command = new CreateWalletCommand(userId, "Main");

        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.WalletId.Should().NotBeEmpty();
        response.Message.Should().Contain("created");
        _walletRepoMock.Verify(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenWalletAlreadyExists_ShouldReturnExisting()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var existingWallet = new Wallet(userId, new WalletTag("Old"));
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingWallet);

        // Act
        var command = new CreateWalletCommand(userId, null);
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.WalletId.Should().Be(existingWallet.Id);
        response.Message.Should().Contain("already exists");
        _walletRepoMock.Verify(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
