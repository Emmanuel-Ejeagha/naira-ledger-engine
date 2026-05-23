using MediatR;
using NairaLedger.Application.Interfaces;
using System.Security.Claims;

namespace NairaLedger.Application.Commands.Auth.Token;

/// <summary>
/// Refreshes access token using a valid refresh token (rotation).
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginUserResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IUserService _userService;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore,
        IUserService userService)
    {
        _tokenService = tokenService;
        _refreshTokenStore = refreshTokenStore;
        _userService = userService;
    }

    public async Task<LoginUserResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.ValidateAccessToken(request.AccessToken);
        var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid access token.");

        var storedUserId = await _refreshTokenStore.ValidateAsync(request.RefreshToken, cancellationToken);
        if (storedUserId is null || storedUserId != userId)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        await _refreshTokenStore.RevokeAsync(request.RefreshToken, cancellationToken);

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        await _refreshTokenStore.StoreAsync(newRefreshToken, user.UserId, DateTime.UtcNow.AddDays(7), cancellationToken);

        return new LoginUserResponse(newAccessToken, newRefreshToken, "Token refreshed.");
    }
}