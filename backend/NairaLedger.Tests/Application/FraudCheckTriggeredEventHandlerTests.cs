namespace NairaLedger.Tests.Application;

/// <summary>
/// Tests the FraudCheckTriggeredEventHandler escalation and logging.
/// </summary>
public class FraudCheckTriggeredEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldLogWarningAndEscalate()
    {
        // Arrange
        var mockEscalation = new Mock<IFraudEscalationService>();
        var mockLogger = new Mock<ILogger<FraudCheckTriggeredEventHandler>>();
        var handler = new FraudCheckTriggeredEventHandler(mockEscalation.Object, mockLogger.Object);

        var walletId = Guid.NewGuid();
        var domainEvent = new FraudCheckTriggeredEvent(walletId, "HighVelocity", "5 transfers in 1 minute");
        var notification = new DomainEventNotification<FraudCheckTriggeredEvent>(domainEvent);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Fraud alert")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);

        mockEscalation.Verify(
            e => e.EscalateAsync(walletId, "HighVelocity", "5 transfers in 1 minute", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}