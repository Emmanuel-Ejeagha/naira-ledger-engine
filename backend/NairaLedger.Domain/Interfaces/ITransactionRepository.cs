using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Domain.Interfaces;


public interface ITransactionRepository
{
    /// <summary>
    /// Retrieves a transaction by its unique identifier.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction.</param>
    /// <returns>The transaction with the specified ID, or null if not found.</returns>
    Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new transaction.
    /// </summary>
    /// <param name="transaction">The transaction to be added.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a transaction with the specified reference exists.
    /// </summary>
    /// <param name="reference">The reference to check for existence.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if a transaction with the specified reference exists; otherwise, false.</returns>
    Task<bool> ExistsByReferenceAsync(TransactionReference reference, CancellationToken cancellationToken = default);
}
