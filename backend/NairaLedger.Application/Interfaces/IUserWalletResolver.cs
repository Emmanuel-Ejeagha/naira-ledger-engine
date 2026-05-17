using NairaLedger.Domain.Aggregates;

namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Resolves the wallet associated with a given user email.
/// </summary>
public interface IUserWalletResolver
{
    Task<Wallet?> GetWalletByEmailAsync(string email, CancellationToken cancellationToken);
}