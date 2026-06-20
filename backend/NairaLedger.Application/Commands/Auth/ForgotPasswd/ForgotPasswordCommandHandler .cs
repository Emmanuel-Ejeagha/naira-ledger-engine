using Microsoft.Extensions.Configuration;

namespace NairaLedger.Application.Commands.Auth.ForgotPasswd;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly string _frontendBaseUrl;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserService userService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userService = userService;
        _emailService = emailService;
        _frontendBaseUrl = configuration["FrontendBaseUrl"] ?? "http://localhost:3000";
        _logger = logger;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            // Don't reveal whether email exists
            _logger.LogInformation("Password reset requested for non‑existent email: {Email}", request.Email);
            return Unit.Value;
        }

        var token = await _userService.GeneratePasswordResetTokenAsync(user.UserId, cancellationToken);
        var encodedToken = Uri.EscapeDataString(token);
        var resetLink = $"{_frontendBaseUrl}/reset-password?userId={user.UserId}&token={encodedToken}&email={Uri.EscapeDataString(user.Email)}";

        await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetLink, cancellationToken);
        _logger.LogInformation("Password reset email sent to {Email}", request.Email);

        return Unit.Value;
    }
}