using System.Security.Claims;

namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Generates and validates JWT tokens and refresh tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates an access token (JWT) for the given claims.
    /// </summary>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Generates a cryptographically strong refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates an access token and returns the claims principal.
    /// Returns null if invalid.
    /// </summary>
    ClaimsPrincipal? ValidateAccessToken(string token);
}