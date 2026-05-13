using NairaLedger.Domain.BaseTypes;

namespace NairaLedger.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in NGN. Enforces non-negative values
/// and ensures all operations are performed in the same currency.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// The currency code, always "NGN. 
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// The amount in Naira (decimal precision)
    /// </summary>
    public decimal Amount { get; }


    /// <summary>
    /// Initializes a new Money instance. Defaults to NGN.
    /// </summary>
    /// <param name="amount">Non-negative amount</param>
    /// <param name="currency">Must be "NGN"</param>
    /// <exception cref="ArgumentException">Throws when amount is negative or currency is invalid</exception>
    public Money(decimal amount, string currency = "NGN")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency must be provided.", nameof(currency));
        if (currency != "NGN")
            throw new ArgumentException("Only NGN currency is supported.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public static Money Zero => new(0);

    public override string ToString() => $"{Currency} {Amount:F2}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }


    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        if (left.Amount < right.Amount)
            throw new InvalidOperationException("Insufficient funds for subtraction.");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static bool operator <(Money left, Money right) => left.Amount < right.Amount;
    public static bool operator >(Money left, Money right) => left.Amount > right.Amount;
    public static bool operator <=(Money left, Money right) => left.Amount <= right.Amount;
    public static bool operator >=(Money left, Money right) => left.Amount >= right.Amount;

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot operate on different currecncies.");
    }
}
