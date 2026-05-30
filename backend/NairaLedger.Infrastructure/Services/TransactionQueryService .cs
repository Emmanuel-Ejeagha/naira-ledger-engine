namespace NairaLedger.Infrastructure.Services;

/// <inheritdoc />
public class TransactionQueryService : ITransactionQueryService
{
    private readonly NairaLedgerDbContext _context;

    public TransactionQueryService(NairaLedgerDbContext context) => _context = context;

    public async Task<PagedResponse<TransactionDto>> GetTransactionsAsync(
        Guid walletId, string? cursor, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<Transaction> query = _context.Transactions
            .Include(t => t.Entries)
            .Where(t => t.Entries.Any(e => e.WalletId == walletId))
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id);

        if (!string.IsNullOrEmpty(cursor))
        {
            var parts = cursor.Split('|');
            if (parts.Length == 2 && DateTime.TryParse(parts[0], out var ts) && Guid.TryParse(parts[1], out var id))
            {
                query = query.Where(t =>
                    t.CreatedAt < ts || (t.CreatedAt == ts && t.Id.CompareTo(id) < 0));
            }
        }

        var transactions = await query.Take(pageSize + 1).ToListAsync(cancellationToken);
        bool hasMore = transactions.Count > pageSize;
        if (hasMore) transactions = transactions.Take(pageSize).ToList();

        string? nextCursor = null;
        if (hasMore)
        {
            var last = transactions.Last();
            nextCursor = $"{last.CreatedAt:O}|{last.Id}";
        }

        var items = transactions.Select(t => new TransactionDto(
            t.Id,
            t.Reference.Value,
            t.Type.ToString(),
            t.Status.ToString(),
            t.Entries.Where(e => e.WalletId == walletId).Sum(e =>
                e.Direction == LedgerEntryDirection.Credit ? e.Amount : -e.Amount),
            t.CreatedAt
        )).ToList();

        return new PagedResponse<TransactionDto>(items, nextCursor, hasMore);
    }
}