using MediatR;
using NairaLedger.Application.Interfaces;
using System.Security.Claims;

namespace NairaLedger.Application.Commands.Auth;

/// <summary>
/// Authenticates a user and returns a JWT + refresh token.
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public LoginUserCommandHandler(
        IUserService userService,
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore)
    {
        _userService = userService;
        _tokenService = tokenService;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.ValidatePasswordAsync(request.Email, request.Password, cancellationToken);
        var user = await _userService.FindByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(7);

        await _refreshTokenStore.StoreAsync(refreshToken, user.UserId, refreshExpiry, cancellationToken);

        return new LoginUserResponse(accessToken, refreshToken, "Login successful.");
    }
}