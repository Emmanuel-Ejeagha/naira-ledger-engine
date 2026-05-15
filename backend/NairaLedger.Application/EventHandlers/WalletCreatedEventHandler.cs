using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Application.Commands;
using NairaLedger.Domain.DomianEvents;

namespace NairaLedger.Application.EventHandlers;

/// <summary>
/// Sends a notification when a new wallet is created.
/// </summary>
public class WalletCreatedEventHandler : INotificationHandler<DomainEventNotification<WalletCreatedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<WalletCreatedEventHandler> _logger;

    public WalletCreatedEventHandler(INotificationService notificationService, ILogger<WalletCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<WalletCreatedEvent> wrapper, CancellationToken cancellationToken)
    {
        var domainEvent = wrapper.DomainEvent;
        _logger.LogInformation("Wallet created: {WalletId}", domainEvent.WalletId);
        await _notificationService.SendToUserAsync(domainEvent.UserId.Value, "Your wallet has been created.", cancellationToken);
    }
}