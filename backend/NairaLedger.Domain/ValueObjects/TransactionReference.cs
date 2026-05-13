using NairaLedger.Domain.BaseTypes;

namespace NairaLedger.Domain.ValueObjects;

/// <summary>
/// Globally unique, human-traceable transaction reference.
/// Format: NW-YYYYMMDD-RANDOM12 (24 chars total, e.g., NW-20240601-1A2B3C4D)
/// </summary>
public sealed class TransactionReference : ValueObject
{
    /// <summary>
    /// The full reference string.
    /// </summary>
    public string Value { get; }

    private static readonly char[] AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    /// <summary>
    /// Creates a reference from an existing string (e.g. from persistence).
    /// </summary>
    /// <param name="value">The reference string.</param>
    /// <exception cref="ArgumentException">Throws when the reference string is null, empty, or not exactly 24 characters long.</exception>
    public TransactionReference(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Transaction reference cannot be empty.", nameof(value));
        if (value.Length != 24)
            throw new ArgumentException("Transaction reference must be exactly 24 characters long.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Generates a new unique refernce for the current date.
    /// </summary>
    /// <returns>A new unique transaction reference.</returns>
    public static TransactionReference Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new string(Enumerable.Range(0, 12)
            .Select(_ => AllowedCharacters[Random.Shared.Next(AllowedCharacters.Length)])
            .ToArray());
        return new TransactionReference($"NW-{datePart}-{random}");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(TransactionReference reference) => reference.Value;
    public override string ToString() => Value;
}
