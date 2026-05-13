namespace NairaLedger.Domain.Enums;

public enum TransactionType
{
    /// <summary>
    /// Wallet funding (e.g., deposit from bank, card, or Paystack transfer)
    /// </summary>
    Funding = 1,
    /// <summary>
    /// Transfer between wallets or accounts
    /// </summary>
    Transfer = 2,
    /// <summary>
    /// Reversal of a previous transaction (e.g., refund, chargeback, or correction of an error)
    /// </summary>
    Reversal = 3,
    /// <summary>
    /// System fee deduction (e.g., transaction fee, service charge, or platform fee)
    /// </summary>
    Fee = 4,
    /// <summary>
    /// Manual adjustment (admin only).
    /// </summary>
    Adjustment = 5
}
