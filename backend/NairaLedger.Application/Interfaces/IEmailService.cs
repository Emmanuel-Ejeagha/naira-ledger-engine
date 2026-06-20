namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Sends transactional emails (credit/debit alerts, KYC notifications).
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a credit alert to the recipient.
    /// </summary>
    Task SendCreditAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a debit alert to the sender.
    /// </summary>
    Task SendDebitAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken cancellationToken);

    /// <summary>
    /// Sends an email verification link to the user after registration or when they request a new verification email.
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="toName"></param>
    /// <param name="verificationLink"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken cancellationToken);
}