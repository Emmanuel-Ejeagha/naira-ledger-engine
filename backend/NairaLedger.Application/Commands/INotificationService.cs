namespace NairaLedger.Application.Commands;

/// <summary>
/// Abstraction for real-time notification (SignalR)
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a specific user.
    /// </summary>
    /// <param name="userId">The unquie identifier of the user</param>
    /// <param name="message">The mesage body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task SendToUserAsync(Guid userId, string message, CancellationToken cancellationToken = default);
}
