namespace NairaWallet.Application.Interfaces;

/// <summary>
/// Abstraction for sending real‑time messages to users, decoupling Infrastructure from SignalR.
/// </summary>
public interface IRealTimeNotifier
{
    /// <summary>
    /// Sends a message to a specific user identified by user ID.
    /// </summary>
    Task SendToUserAsync(Guid userId, string message, CancellationToken cancellationToken = default);
}