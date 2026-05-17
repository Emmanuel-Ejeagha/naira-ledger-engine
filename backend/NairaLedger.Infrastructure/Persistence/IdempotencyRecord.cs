namespace NairaLedger.Infrastructure.Persistence;

/// <summary>
/// Stores the outcome of an idempotent operation.
/// </summary>
public class IdempotencyRecord
{
    public string Key { get; set; } = default!;
    public string ResponseData { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}