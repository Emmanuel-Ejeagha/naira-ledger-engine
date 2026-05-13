using NairaLedger.Domain.BaseTypes;

namespace NairaLedger.Domain.ValueObjects;

public sealed class IdempotencyKey : ValueObject
{
    public string Value { get; }

    public IdempotencyKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Idempotency key cannot be empty.", nameof(value));
        if (value.Length > 128)
            throw new ArgumentException("Idempotency key cannot exceed 128 characters.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(IdempotencyKey key) => key.Value;
    public static explicit operator IdempotencyKey(string value) => new(value);
}
