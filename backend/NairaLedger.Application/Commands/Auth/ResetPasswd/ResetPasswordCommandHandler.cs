using System;
using System.Collections.Generic;
using System.Text;

namespace NairaLedger.Application.Commands.Auth.ResetPasswd;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(IUserService userService, ILogger<ResetPasswordCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var decodedToken = Uri.UnescapeDataString(request.Token);
        await _userService.ResetPasswordAsync(request.UserId, decodedToken, request.NewPassword, cancellationToken);
        _logger.LogInformation("Password reset successful for user {UserId}", request.UserId);
        return Unit.Value;
    }
}
