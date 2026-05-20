using Microsoft.EntityFrameworkCore;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Enums;
using NairaLedger.Infrastructure.Persistence;

namespace NairaLedger.Infrastructure.Services;

/// <inheritdoc />
public class LedgerQueryService : ILedgerQueryService
{
    private readonly NairaLedgerDbContext _context;

    public LedgerQueryService(NairaLedgerDbContext context) => _context = context;

    public async Task<decimal> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var credits = await _context.LedgerEntries
            .Where(e => e.WalletId == walletId && e.Direction == LedgerEntryDirection.Credit)
            .SumAsync(e => e.Amount, cancellationToken);

        var debits = await _context.LedgerEntries
            .Where(e => e.WalletId == walletId && e.Direction == LedgerEntryDirection.Debit)
            .SumAsync(e => e.Amount, cancellationToken);

        return credits - debits;
    }
}