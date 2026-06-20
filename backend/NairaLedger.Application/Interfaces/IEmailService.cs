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

    /// <summary>
    /// Sends a password reset link to the user when they request a password reset.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="resetLink">The link to reset the password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email to the user notifying them that their wallet has been funded.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="amount">The amount funded.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendWalletFundedEmailAsync(string toEmail, string toName, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email to the user notifying them that their wallet has been frozen, along with the reason for the freeze.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="reason">The reason for the freeze.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendWalletFrozenEmailAsync(string toEmail, string toName, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email to the user notifying them that their KYC has been approved.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendKycApprovedEmailAsync(string toEmail, string toName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email to the user notifying them that their KYC has been rejected, along with the reason for rejection.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="reason">The reason for the rejection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendKycRejectedEmailAsync(string toEmail, string toName, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email to the user notifying them that their password has been changed successfully.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPasswordChangedEmailAsync(string toEmail, string toName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends a welcome email to the user after successful registration.
    /// </summary>
    /// <param name="toEmail">The email address of the user.</param>
    /// <param name="toName">The name of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendWelcomeEmailAsync(string toEmail, string toName, CancellationToken cancellationToken = default);
}