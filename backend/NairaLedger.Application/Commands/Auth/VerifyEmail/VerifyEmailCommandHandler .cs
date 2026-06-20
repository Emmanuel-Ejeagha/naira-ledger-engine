namespace NairaLedger.Application.Commands.Auth.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(IUserService userService, IEmailService emailService, ILogger<VerifyEmailCommandHandler> logger)
    {
        _userService = userService;
        _emailService = emailService;
        _logger = logger;
    }
    public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken) ?? throw new InvalidOperationException("User not found.");
        if (user.EmailConfirmed)
            throw new InvalidOperationException("Email is already verified.");

        await _userService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);
        _logger.LogInformation("Email verified for user {UserId}", request.UserId);

        try
        {
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName, cancellationToken);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to send welcome email"); }
        return Unit.Value;
    }
}