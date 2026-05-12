using NairaLedger.Domain.BaseTypes;

namespace NairaLedger.Domain.ValueObjects;

/// <summary>
/// Uniquely identifies a user in the system. Wraps Guid for type safety.
/// </summary>
public sealed class UserId : ValueObject
{
    /// <summary>
    /// The underlying user identified GUID.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a UserId. Must not be empty Guid.
    /// </summary>
    /// <param name="value">The GUID value to wrap.</param>
    /// <exception cref="ArgumentException">Thrown when the provided GUID is empty.</exception>
    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(value));
        Value = value;
    }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => new(value);
}
