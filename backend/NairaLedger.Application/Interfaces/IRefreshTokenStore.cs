namespace NairaWallet.Application.Interfaces;

/// <summary>
/// Persists refresh tokens for token rotation and revocation.
/// </summary>
public interface IRefreshTokenStore
{
    /// <summary>
    /// Stores a refresh token for the given user with an expiration.
    /// </summary>
    Task StoreAsync(string token, Guid userId, DateTime expiresAt, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the refresh token is valid (exists and not expired/revoked).
    /// Returns the associated user ID, or null if invalid.
    /// </summary>
    Task<Guid?> ValidateAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes a specific refresh token.
    /// </summary>
    Task RevokeAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes all refresh tokens for a given user.
    /// </summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken);
}