namespace NairaLedger.Domain.Enums;

public enum KycLevel
{
    /// <summary> Not Verified.</summary>
    Unverified = 0,
    /// <summary> Basic identity captured.</summary>
    Tier1 = 1,
    /// <summary> Government ID verified.</summary>
    Tier2 = 2,
    /// <summary>Proof of address verified.</summary>
    Tier3 = 3,
}
