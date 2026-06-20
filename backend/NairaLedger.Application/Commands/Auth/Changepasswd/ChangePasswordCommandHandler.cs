using Microsoft.AspNetCore.Http;

namespace NairaLedger.Application.Commands.Auth;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor, IEmailService emailService, ILogger<ChangePasswordCommandHandler> logger)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException();

        await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);

        try
        {
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
                await _emailService.SendPasswordChangedEmailAsync(user.Email, user.FullName, cancellationToken);
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Failed to send password change email"); }
    }
}