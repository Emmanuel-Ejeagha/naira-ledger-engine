using MediatR;
using Microsoft.Extensions.Logging;
using NairaLedger.Application.EventHandlers;
using NairaLedger.Domain.DomianEvents;
using NairaWallet.Application.Interfaces;

namespace NairaWallet.Application.EventHandlers;

/// <summary>
/// Logs and escalates fraud alerts via the IFraudEscalationService.
/// </summary>
public class FraudCheckTriggeredEventHandler : INotificationHandler<DomainEventNotification<FraudCheckTriggeredEvent>>
{
    private readonly IFraudEscalationService _escalationService;
    private readonly ILogger<FraudCheckTriggeredEventHandler> _logger;

    public FraudCheckTriggeredEventHandler(
        IFraudEscalationService escalationService,
        ILogger<FraudCheckTriggeredEventHandler> logger)
    {
        _escalationService = escalationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<FraudCheckTriggeredEvent> wrapper, CancellationToken cancellationToken)
    {
        var domainEvent = wrapper.DomainEvent;
        _logger.LogWarning("Fraud alert: {Rule} on wallet {WalletId}. {Description}",
            domainEvent.RuleName, domainEvent.WalletId, domainEvent.Description);

        await _escalationService.EscalateAsync(
            domainEvent.WalletId,
            domainEvent.RuleName,
            domainEvent.Description,
            cancellationToken);
    }
}