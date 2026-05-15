using Microsoft.Extensions.Logging;
using Moq;
using NairaLedger.Application.Commands;
using NairaLedger.Application.Commands.EventHandlers;
using NairaLedger.Application.EventHandlers;
using NairaLedger.Domain.DomianEvents;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests.Application;

/// <summary>
/// Tests the WalletCreatedEventHandler notification flow.
/// </summary>
public class WalletCreatedEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSendNotificationToUser()
    {
        // Arrange
        var mockNotification = new Mock<INotificationService>();
        var mockLogger = new Mock<ILogger<WalletCreatedEventHandler>>();
        var handler = new WalletCreatedEventHandler(mockNotification.Object, mockLogger.Object);

        var walletId = Guid.NewGuid();
        var userId = new UserId(Guid.NewGuid());
        var domainEvent = new WalletCreatedEvent(walletId, userId);
        var notification = new DomainEventNotification<WalletCreatedEvent>(domainEvent);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        mockNotification.Verify(
            n => n.SendToUserAsync(userId.Value, It.Is<string>(s => s.Contains("wallet")), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}