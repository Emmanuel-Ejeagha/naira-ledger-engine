namespace NairaLedger.Application.Commands.Auth.Login;

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
        var user = await _userService.FindByEmailAsync(request.Email, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.EmailConfirmed)
            throw new InvalidOperationException("Please verify your email address before logging in.");

        // Build claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName)
        };
        claims.Add(new Claim("email_verified", user.EmailConfirmed.ToString().ToLower()));

        // Add role claims
        var roles = await _userService.GetRolesAsync(user.UserId, cancellationToken);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(7);

        await _refreshTokenStore.StoreAsync(refreshToken, user.UserId, refreshExpiry, cancellationToken);

        return new LoginUserResponse(accessToken, refreshToken, "Login successful.");
    }
}