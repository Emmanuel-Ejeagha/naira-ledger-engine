namespace NairaLedger.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository, IUserWalletResolver
{
    private readonly NairaLedgerDbContext _context;

    public WalletRepository(NairaLedgerDbContext context) => _context = context;

    public async Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
        => await _context.Wallets.FindAsync(new object[] { walletId }, cancellationToken);

    public async Task<Wallet?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken)
        => await _context.Wallets.FirstOrDefaultAsync(w => w.UserId.Equals(userId), cancellationToken);

    public async Task AddAsync(Wallet wallet, CancellationToken cancellationToken)
        => await _context.Wallets.AddAsync(wallet, cancellationToken);

    public Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        _context.Wallets.Update(wallet);
        return Task.CompletedTask;
    }

    public async Task<WalletOwnerInfo?> GetOwnerInfoAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallets
            .Include(w => w.UserId) 
            .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);

        if (wallet is null) return null;

        var user = await _context.Users.FindAsync(new object[] { wallet.UserId.Value }, cancellationToken);
        if (user is null) return null;

        return new WalletOwnerInfo(user.Email ?? "", user.FullName);
    }

    public async Task<Wallet?> GetWalletByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null) return null;

        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId.Equals(new UserId(user.Id)), cancellationToken);
    }
}