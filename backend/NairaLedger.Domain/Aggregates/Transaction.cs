using NairaLedger.Domain.BaseTypes;
using NairaLedger.Domain.DomianEvents;
using NairaLedger.Domain.Entities;
using NairaLedger.Domain.Enums;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Domain.Aggregates;

/// <summary>
/// Aggregate root representing a financial transaction.
/// A transaction MUST contain at least two ledger entries and MUST balance
/// (total debits == total credits). Once created as Completed it is immutable;
/// corrections are only allowed via reversal transactions. Reversals can only be created within 30 minutes of the original transaction and cannot reverse another reversal.
/// </summary>
public class Transaction : AggregateRoot
{
    private readonly List<LedgerEntry> _entries = new();

    /// <summary>
    /// Unique, human-readble refernce for external traceability (e.g. "NW-20240601-1A2B3C4D").
    /// </summary>
    public TransactionReference Reference { get; private set; } = default!;

    /// <summary>
    /// The business type of this transaction.
    /// </summary>
    public TransactionType Type { get; private set; }

    /// <summary>
    /// Current lifecycle status of the transaction.
    /// </summary>
    public TransactionStatus Status { get; private set; }

    /// <summary>
    /// UTC timestamp when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// The user who initiated the transacction (if applicable)
    /// </summary>
    public Guid? InitiatedByUserId { get; private set; }

    /// <summary>
    /// Read-only collection of Ledger entries that constitute this transaction.
    /// </summary>
    public IReadOnlyCollection<LedgerEntry> Entries => _entries.AsReadOnly();

    private Transaction() { }

    /// <summary>
    /// Creates a n ew transaction that is immediately Completed.
    /// Validates balancing and raises the appropriate domain event based on type.
    /// </summary>
    /// <param name="reference">The human-readable reference for the transaction.</param>
    /// <param name="type">The business type of the transaction.</param>
    /// <param name="entries">The ledger entries that constitute the transaction.</param>
    /// <param name="initiatedByUserId">The user who initiated the transaction (if applicable).</param>
    /// <exception cref="ArgumentException">Thrown when the entry list is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when debits do not equal credits.</exception>
    public Transaction(
    TransactionReference reference,
        TransactionType type,
        List<LedgerEntry> entries,
        Guid? initiatedByUserId)
    {
        if (entries == null || entries.Count < 2)
            throw new ArgumentException("A transaction must have at least two ledger entries.");
        if (type == TransactionType.Transfer && entries.Count != 2)
            throw new ArgumentException("Transfer transactions must have exactly two ledger entries.");

        var totalDebits = entries
            .Where(e => e.Direction == LedgerEntryDirection.Debit)
            .Sum(e => e.Amount);
        var totalCredits = entries
            .Where(e => e.Direction == LedgerEntryDirection.Credit)
            .Sum(e => e.Amount);

        if (totalDebits != totalCredits)
            throw new InvalidOperationException(
                $"Transaction does not balance: Debits = {totalDebits}, Credits = {totalCredits}");

        Id = Guid.NewGuid();
        Reference = reference;
        Type = type;
        Status = TransactionStatus.Completed;
        _entries.AddRange(entries);
        CreatedAt = DateTime.UtcNow;
        InitiatedByUserId = initiatedByUserId;

        RaiseTypedEvent();
    }

    /// <summary>
    /// Creates a reversal transaction that negates the effects of the original transaction.
    /// The reversal window is 30 minutes from the original transaction's creation time. Reversals cannot be created for transactions that are not Completed or for other Reversal transactions.
    /// </summary>
    /// <param name="original">The original transaction to be reversed.</param>
    /// <param name="initiatedBy">The user who initiated the reversal (if applicable).</param>
    /// <returns>The newly created reversal transaction.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the original transaction is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the original transaction cannot be reversed.</exception>
    public static Transaction CreateReversal(Transaction original, Guid? initiatedBy)
    {
        if (original == null)
            throw new ArgumentNullException(nameof(original));
        if (original.Status != TransactionStatus.Completed)
            throw new InvalidOperationException("Only completed transactions can be reversed.");
        if (DateTime.UtcNow - original.CreatedAt > TimeSpan.FromMinutes(30))
            throw new InvalidOperationException("Transactions older than 30 minutes have expired.");
        if (original.Type == TransactionType.Reversal)
            throw new InvalidOperationException("Reversal transactions cannot be reversed again.");

        var reversedEntries = original.Entries
            .Select(entry => new LedgerEntry(
                entry.WalletId,
                entry.Amount,
                entry.Direction == LedgerEntryDirection.Debit
                    ? LedgerEntryDirection.Credit
                    : LedgerEntryDirection.Debit,
                $"Reversal of {original.Reference.Value}"
            )).ToList();

        var reversal = new Transaction(
            TransactionReference.Generate(),
            TransactionType.Reversal,
            reversedEntries,
            initiatedBy);

        reversal.AddDomainEvent(new TransactionReversedEvent(reversal.Id, original.Id, $"Reversal of transaction {original.Reference.Value}"));

        return reversal;
    }


    /// <summary>
    /// Raises a domain event corresponding to the current transaction type.    
    /// </summary>
    /// <remarks>This method determines the transaction type and raises the appropriate domain event, such as
    /// funding or transfer events. It should be called when the transaction state changes and an event notification is
    /// required. The method assumes that the necessary ledger entries are present and valid for the transaction
    /// type.</remarks>
    private void RaiseTypedEvent()
    {
        switch (Type)
        {
            case TransactionType.Funding:
                {
                    var creditEntry = _entries.First(e => e.Direction == LedgerEntryDirection.Credit);
                    AddDomainEvent(new WalletFundedEvent(Id, creditEntry.WalletId, creditEntry.Amount));
                    break;
                }
            case TransactionType.Transfer:
                {
                    var debitEntry = _entries.First(e => e.Direction == LedgerEntryDirection.Debit);
                    var creditEntry = _entries.First(e => e.Direction == LedgerEntryDirection.Credit);
                    AddDomainEvent(new TransferCompletedEvent(Id, debitEntry.WalletId, creditEntry.WalletId, debitEntry.Amount));
                    break;
                }
        }
    }
}