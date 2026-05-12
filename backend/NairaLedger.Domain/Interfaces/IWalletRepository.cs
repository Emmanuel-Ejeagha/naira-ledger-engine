using NairaLedger.Domain.Aggregates;

namespace NairaLedger.Domain.Interfaces;

/// <summary>
/// Abstactions for wallet persistence.
/// </summary>
public interface IWalletRepository
{
    /// <summary>
    /// Retrieves a wallet by its primary key
    /// </summary>
    /// <param name="walletId">The unique identifier of the wallet.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The wallet if found; otherwise, null.</returns>
    Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a wallet owned by a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The wallet if found; otherwise, null.</returns>
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new wallet to the repository.
    /// </summary>
    /// <param name="wallet">The wallet to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>    
    Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to an existing wallet in the repository.
    /// </summary>
    /// <param name="wallet">The wallet to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default);
}
