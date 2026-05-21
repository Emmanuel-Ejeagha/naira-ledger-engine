using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Application.Commands;
using NairaLedger.Application.EventHandlers;
using NairaLedger.Domain.DomianEvents;

namespace NairaLedger.Tests.Application;

/// <summary>
/// Tests the TransferCompletedEventHandler notification flow.
/// </summary>
public class TransferCompletedEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldNotifyBothParties()
    {
        // Arrange
        var mockNotification = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<TransferCompletedEventHandler>>();
        var handler = new TransferCompletedEventHandler(mockNotification.Object, mockLogger.Object);

        var fromWallet = Guid.NewGuid();
        var toWallet = Guid.NewGuid();
        var domainEvent = new TransferCompletedEvent(Guid.NewGuid(), fromWallet, toWallet, 500);
        var notification = new DomainEventNotification<TransferCompletedEvent>(domainEvent);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        mockNotification.Verify(
            n => n.SendToUserAsync(fromWallet, "Transfer sent.", It.IsAny<CancellationToken>()),
            Times.Once);
        mockNotification.Verify(
            n => n.SendToUserAsync(toWallet, "Transfer received.", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}