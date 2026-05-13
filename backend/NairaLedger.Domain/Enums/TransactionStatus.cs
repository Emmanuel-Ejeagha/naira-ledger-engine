namespace NairaLedger.Domain.Enums;

public enum TransactionStatus
{
    /// <summary>
    /// Transaction is pending and not yet completed.
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Transaction has been successfully completed.
    /// </summary>
    Completed = 2,
    /// <summary>
    /// Transaction has been failed.
    /// </summary>
    Failed = 3,
    /// <summary>
    /// Transaction has been reversed.
    /// </summary>
    Reversed = 4
}