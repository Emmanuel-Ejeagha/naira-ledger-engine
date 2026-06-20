
namespace NairaLedger.Infrastructure.Services;

/// <inheritdoc />
public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendCreditAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken cancellationToken)
    {
        var subject = $"Credit Alert: You received NGN {amount:N2}";
        var body = $"Dear {toName},\n\nYou have received NGN {amount:N2} from a transfer (Ref: {reference}).\n\nYour wallet has been credited.\n\nThank you.";
        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendDebitAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken cancellationToken)
    {
        var subject = $"Debit Alert: You sent NGN {amount:N2}";
        var body = $"Dear {toName},\n\nYou have sent NGN {amount:N2} to another wallet (Ref: {reference}).\n\nYour wallet has been debited.\n\nThank you.";
        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };
        var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(to);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken cancellationToken)
    {
        var subject = "Verify your NairaLedger email address";
        var body = $@"<p>Hi {toName},</p>
            <p>Please verify your email address by clicking the link below:</p>
            <p><a href=""{verificationLink}"">Verify Email</a></p>
            <p>If you didn't create an account, please ignore this email.</p>";
        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink, CancellationToken cancellationToken = default)
    {
        var subject = "Reset your NairaLedger password";
        var body = $@"<p>Hi {toName},</p>
            <p>You requested a password reset. Click the link below to reset your password:</p>
            <p><a href=""{resetLink}"">Reset Password</a></p>
            <p>If you didn't request this, please ignore this email.</p>";
        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.mailtrap.io";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "no-reply@nairawallet.ng";
    public string FromName { get; set; } = "NairaWallet";
}