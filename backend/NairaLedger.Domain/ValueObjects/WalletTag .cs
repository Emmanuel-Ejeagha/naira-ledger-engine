using NairaLedger.Domain.BaseTypes;

namespace NairaLedger.Domain.ValueObjects;

/// <summary>
/// A user-defined lable for a wallet (e.g, "Savings", "Current").
/// Enforces length limit and trims whitespace
/// </summary>
public sealed class WalletTag : ValueObject
{
    /// <summary>
    /// The trimmed tag value.
    /// </summary>
    public string Value { get; }

    public WalletTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wallet tag cannot be empty.", nameof(value));
        if (value.Length > 50)
            throw new ArgumentException("Wallet tag cannot exceed 50 characters.", nameof(value));

        Value = value.Trim();        
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(WalletTag tag) => tag.Value;
    public static implicit operator WalletTag(string value) => new(value);
}
