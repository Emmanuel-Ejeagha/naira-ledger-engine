using NairaLedger.Domain.ValueObjects;

namespace NairaWallet.Application.Interfaces;

/// <summary>
/// Persistence abstraction for idempotency records.
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    /// Checks whether the given idempotency key has already been processed.
    /// Returns the stored response if found, otherwise null.
    /// </summary>
    Task<IdempotentResponse?> GetResponseAsync(IdempotencyKey key, CancellationToken cancellationToken);

    /// <summary>
    /// Stores the response for a newly processed idempotency key.
    /// </summary>
    Task StoreResponseAsync(IdempotencyKey key, IdempotentResponse response, CancellationToken cancellationToken);
}

/// <summary>
/// Captures the outcome of an idempotent operation.
/// </summary>
public record IdempotentResponse(object? Result, string? Error);