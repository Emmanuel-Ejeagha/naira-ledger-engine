using Microsoft.AspNetCore.SignalR;
using NairaLedger.Application.Interfaces;
using NairaLedger.WebApi.Hubs;

namespace NairaLedger.WebApi.Services;

public class SignalRRealTimeNotifier : IRealTimeNotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRRealTimeNotifier(IHubContext<NotificationHub> hubContext) => _hubContext = hubContext;

    public async Task SendToUserAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message, cancellationToken);
    }
}