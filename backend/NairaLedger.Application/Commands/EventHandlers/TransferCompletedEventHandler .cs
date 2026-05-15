using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Application.Commands;
using NairaLedger.Application.Commands.EventHandlers;
using NairaLedger.Domain.DomianEvents;
using NairaWallet.Application.Interfaces;

namespace NairaWallet.Application.EventHandlers;

/// <summary>
/// Sends a notification when a transfer completes.
/// </summary>
public class TransferCompletedEventHandler : INotificationHandler<DomainEventNotification<TransferCompletedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TransferCompletedEventHandler> _logger;

    public TransferCompletedEventHandler(INotificationService notificationService, ILogger<TransferCompletedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<TransferCompletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation("Transfer completed: {From} -> {To}, Amount: {Amount}",
            domainEvent.FromWalletId, domainEvent.ToWalletId, domainEvent.Amount);
        // Notify both parties
        await _notificationService.SendToUserAsync(domainEvent.FromWalletId, "Transfer sent.", cancellationToken);
        await _notificationService.SendToUserAsync(domainEvent.ToWalletId, "Transfer received.", cancellationToken);
    }
}