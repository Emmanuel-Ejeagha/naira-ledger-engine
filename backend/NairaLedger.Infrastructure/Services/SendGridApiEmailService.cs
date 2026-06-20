using System.Net.Http.Json;
using System.Text.Json;
using NairaLedger.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace NairaLedger.Infrastructure.Services;

public class SendGridApiEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _fromAddress;
    private readonly string _fromName;
    private readonly ILogger<SendGridApiEmailService> _logger;

    public SendGridApiEmailService(
        HttpClient httpClient,
        IOptions<SmtpSettings> smtpOptions,
        ILogger<SendGridApiEmailService> logger)
    {
        _httpClient = httpClient;
        _fromAddress = smtpOptions.Value.FromAddress;
        _fromName = smtpOptions.Value.FromName;
        _logger = logger;
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string html, CancellationToken ct)
    {
        var payload = new
        {
            personalizations = new[]
            {
                new
                {
                    to = new[] { new { email = toEmail, name = toName } },
                    subject
                }
            },
            from = new { email = _fromAddress, name = _fromName },
            content = new[]
            {
                new { type = "text/html", value = html }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("mail/send", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("SendGrid API error: {Error}", error);
            throw new InvalidOperationException($"Email sending failed: {response.ReasonPhrase}");
        }
    }

    public Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p>"
                 + $"<p>Please verify your email by clicking the link below:</p>"
                 + $"<p><a href=\"{verificationLink}\">Verify Email</a></p>"
                 + $"<p>If you didn't create an account, please ignore this email.</p>";
        return SendAsync(toEmail, toName, "Verify your NairaLedger email", html, ct);
    }

    public Task SendWalletFundedEmailAsync(string toEmail, string toName, decimal amount, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Your wallet has been credited with NGN {amount:N2}.</p>";
        return SendAsync(toEmail, toName, "Wallet Funded", html, ct);
    }

    public Task SendWalletFrozenEmailAsync(string toEmail, string toName, string reason, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Your wallet has been temporarily frozen due to: {reason}.</p><p>Please contact support.</p>";
        return SendAsync(toEmail, toName, "Wallet Frozen", html, ct);
    }

    public Task SendKycApprovedEmailAsync(string toEmail, string toName, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Congratulations! Your KYC verification has been approved.</p>";
        return SendAsync(toEmail, toName, "KYC Approved", html, ct);
    }

    public Task SendKycRejectedEmailAsync(string toEmail, string toName, string reason, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Your KYC verification was not approved. Reason: {reason}.</p>";
        return SendAsync(toEmail, toName, "KYC Rejected", html, ct);
    }

    public Task SendPasswordChangedEmailAsync(string toEmail, string toName, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Your password was changed successfully. If this wasn't you, please reset your password immediately.</p>";
        return SendAsync(toEmail, toName, "Password Changed", html, ct);
    }

    public Task SendWelcomeEmailAsync(string toEmail, string toName, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p><p>Welcome to NairaLedger! Your email has been verified and your account is now active.</p>";
        return SendAsync(toEmail, toName, "Welcome to NairaLedger!", html, ct);
    }

    public Task SendCreditAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken ct)
    {
        var html = $"<p>Dear {toName},</p><p>You have received NGN {amount:N2} (Ref: {reference}).</p>";
        return SendAsync(toEmail, toName, "Credit Alert", html, ct);
    }

    public Task SendDebitAlertAsync(string toEmail, string toName, decimal amount, string reference, CancellationToken ct)
    {
        var html = $"<p>Dear {toName},</p><p>You have sent NGN {amount:N2} (Ref: {reference}).</p>";
        return SendAsync(toEmail, toName, "Debit Alert", html, ct);
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink, CancellationToken ct)
    {
        var html = $"<p>Hi {toName},</p>"
                 + $"<p>Click the link below to reset your password:</p>"
                 + $"<p><a href=\"{resetLink}\">Reset Password</a></p>"
                 + $"<p>If you didn't request this, please ignore this email.</p>";
        return SendAsync(toEmail, toName, "Password Reset", html, ct);
    }
}