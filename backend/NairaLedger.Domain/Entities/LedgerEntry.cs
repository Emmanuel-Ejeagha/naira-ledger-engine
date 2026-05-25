namespace NairaLedger.Domain.Entities;

/// <summary>
/// A single entry in the double-entry ledger. Always part of a Transaction aggregate.
/// It debits or credits exactly one wallet with a positive amount.
/// </summary>
public class LedgerEntry : Entity
{
    /// <summary>
    /// The wallet that this entry affects.
    /// </summary>
    public Guid WalletId { get; private set; }

    /// <summary>
    /// The positve monetary amount
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Debit (decrease wallet) or Credit (increase wallet)
    /// </summary>
    public LedgerEntryDirection Direction { get; private set; }

    /// <summary>
    /// Human-readable description of the entry.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// UTC timestamp when the entry was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private LedgerEntry() { }

    /// <summary>
    /// Creates a valid Ledger entry. The amount must be positive, and the wallet ID cannot be empty.
    /// </summary>
    /// <param name="walletId">The ID of the wallet affected by this entry.</param>
    /// <param name="amount">The positive monetary amount of the entry.</param>
    /// <param name="direction">The direction of the entry, either Debit or Credit.</param>
    /// <param name="description">A human-readable description of the entry.</param>
    /// <exception cref="ArgumentException">Thrown when the wallet ID is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the amount is not positive.</exception>
    public LedgerEntry(Guid walletId, decimal amount, LedgerEntryDirection direction, string? description)
    {
        if (walletId == Guid.Empty)
            throw new ArgumentException("Wallet ID cannot be empty.", nameof(walletId));
        if (amount <= 0) 
            throw new ArgumentException(nameof(amount), "Amount must be positive");

        Id = Guid.NewGuid();
        WalletId = walletId;
        Amount = amount;
        Direction = direction;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

}
