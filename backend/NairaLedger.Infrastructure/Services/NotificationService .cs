using NairaLedger.Application.Commands;
using NairaLedger.Application.Interfaces;

namespace NairaLedger.Infrastructure.Services;

/// <summary>
/// Sends real‑time notifications to users via the injected IRealTimeNotifier (SignalR in production).
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IRealTimeNotifier _notifier;

    public NotificationService(IRealTimeNotifier notifier) => _notifier = notifier;

    public async Task SendToUserAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {
        await _notifier.SendToUserAsync(userId, message, cancellationToken);
    }
}