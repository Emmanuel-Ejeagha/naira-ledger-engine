using Microsoft.Extensions.Configuration;

namespace NairaLedger.Application.Commands.Auth.VerifyEmail;

public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly string _frontendBaseUrl;
    private readonly ILogger<SendVerificationEmailCommandHandler> _logger;

    public SendVerificationEmailCommandHandler(
        IUserService userService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<SendVerificationEmailCommandHandler> logger)
    {
        _userService = userService;
        _emailService = emailService;
        _frontendBaseUrl = configuration["FrontendBaseUrl"] ?? "http://localhost:3000";
        _logger = logger;
    }

    public async Task<Unit> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByEmailAsync(request.Email, cancellationToken) ?? throw new InvalidOperationException("User not found.");
        if (user.EmailConfirmed)
            throw new InvalidOperationException("Email is already verified.");

        var token = await _userService.GenerateEmailConfirmationTokenAsync(user.UserId, cancellationToken);
        var encodedToken = Uri.EscapeDataString(token);
        var verificationLink = $"{_frontendBaseUrl}/verify-email?userId={user.UserId}&token={encodedToken}";

        await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationLink, cancellationToken);
        _logger.LogInformation("Verification email sent to {Email}", request.Email);

        return Unit.Value;
    }
}