using Microsoft.EntityFrameworkCore;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Infrastructure.Persistence.Repositories;

/// <inheritdoc />
public class TransactionRepository : ITransactionRepository
{
    private readonly NairaLedgerDbContext _context;

    public TransactionRepository(NairaLedgerDbContext context) => _context = context;

    public async Task<Transaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken) =>
        await _context.Transactions.FindAsync(new object[] { transactionId }, cancellationToken);

    public async Task<Transaction?> GetByIdWithEntriesAsync(Guid transactionId, CancellationToken cancellationToken) =>
        await _context.Transactions.Include(t => t.Entries).FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken) =>
        await _context.Transactions.AddAsync(transaction, cancellationToken);

    public async Task<bool> ExistsByReferenceAsync(TransactionReference reference, CancellationToken cancellationToken) =>
        await _context.Transactions.AnyAsync(t => t.Reference.Equals(reference), cancellationToken);
}