using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests;

/// <summary>
/// Provides test‑friendly wallet instances with controllable ID and active state.
/// Uses reflection because the domain model intentionally keeps setters private.
/// </summary>
public static class TestWalletFactory
{
    public static Wallet Create(Guid id, bool isActive = true)
    {
        var wallet = new Wallet(new UserId(Guid.NewGuid()));
        typeof(Wallet).GetProperty(nameof(Wallet.Id))!.SetValue(wallet, id);
        typeof(Wallet).GetProperty(nameof(Wallet.IsActive))!.SetValue(wallet, isActive);

        return wallet;
    }
}
