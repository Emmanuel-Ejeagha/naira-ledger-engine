namespace NairaLedger.Domain.Aggregates;

/// <summary>
/// Represent's a user's digital wallet for holdoing NGN.
/// The wallet itself does NOT maintain a cached balance;
/// the balance is derived from immutable Ledger.
/// It enforces KYC levels progression (no downgrades) and active/inactive state.
/// </summary>
public class Wallet : AggregateRoot
{
    /// <summary>
    /// The user who owns this wallet.
    /// </summary>
    public UserId UserId { get; private set; } = default!;

    /// <summary>
    /// An optional user-defined tag
    /// </summary>
    public WalletTag? Tag { get; private set; }

    /// <summary>
    /// Current KYC verification level of the wallet owner
    /// </summary>
    public KycLevel KycLevel { get; private set; } = KycLevel.Unverified;

    /// <summary>
    /// Full name submitted during KYC.
    /// </summary>
    public string? KycFullName { get; private set; }

    /// <summary>
    /// ID number submitted during KYC.
    /// </summary>
    public string? KycIdNumber { get; private set; }

    /// <summary>
    /// Type of ID submitted (e.g., National ID, Passport).
    /// </summary>
    public string? KycIdType { get; private set; }

    /// <summary>
    /// Indicates whether the wallet is currently active. 
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// UTC timestamp of wallet creation.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Concurrency token for optimistic locking (set by persistence).
    /// </summary>
    public Guid Version { get; private set; }

    private Wallet() { }

    /// <summary>
    /// Creates a new wallet for a given user.
    /// </summary>
    /// <param name="userId">Owner identity.</param>
    /// <param name="tag">Optional wallet label</param>
    public Wallet(UserId userId, WalletTag? tag = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Tag = tag;
        KycLevel = KycLevel.Unverified;
        CreatedAt = DateTime.UtcNow;
        Version = Guid.NewGuid();

        AddDomainEvent(new WalletCreatedEvent(Id, UserId));
    }


    /// <summary>
    /// Upgrades the KYC Level. Level can only be upgraded, not downgraded.
    /// </summary>
    /// <param name="newLevel">Must be higher than current level.</param>
    /// <exception cref="InvalidOperationException">Thrown when attempting to downgrade</exception>
    public void VerifyKyc(KycLevel newLevel)
    {
        if (newLevel <= KycLevel)
            throw new InvalidOperationException($"Cannot downgrade KYC level from {KycLevel} to {newLevel}.");

        KycLevel = newLevel;
        AddDomainEvent(new KycVerifiedEvent(Id, newLevel));
    }

    /// <summary>
    /// Marks the wallet as inactive.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    /// <summary>
    /// Reactivates a previously deactivated wallet. 
    /// </summary>
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    /// <summary>
    /// Rejects a KYC submission and resets the KYC level to Unverified.
    /// </summary>
    public void RejectKyc()
    {
        KycLevel = KycLevel.Unverified;
        UpdateVersion();
    }

    /// <summary>
    /// Bump the concurrency token whenever any state changes.
    /// </summary>
    private void UpdateVersion()
    {
        Version = Guid.NewGuid();
    }

    public void SubmitKyc(string fullName, string idNumber, string idType)
    {
        if (KycLevel != KycLevel.Unverified)
            throw new InvalidOperationException("KYC has already been submitted.");

        KycFullName = fullName;
        KycIdNumber = idNumber;
        KycIdType = idType;
        KycLevel = KycLevel.Tier1;
        UpdateVersion();
    }
}