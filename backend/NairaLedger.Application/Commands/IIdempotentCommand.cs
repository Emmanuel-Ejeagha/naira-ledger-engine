namespace NairaLedger.Application.Commands;

/// <summary>
/// Maker interface for commands that require idempotency enforcement.
/// </summary>
public interface IIdempotentCommand
{
    IdempotencyKey IdempotencyKey { get; }
}
