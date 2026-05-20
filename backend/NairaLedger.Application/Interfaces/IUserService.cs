namespace NairaLedger.Application.Interfaces;

/// <summary>
/// Abstraction for identity operations, decoupling Application from ASP.NET Identity.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user with the given email, full name, and password.
    /// Returns the user ID or throws if creation fails.
    /// </summary>
    Task<CreateUserResult> CreateUserAsync(string email, string fullName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by email and returns a simplified user representation, or null.
    /// </summary>
    Task<UserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the password for the given email. Throws UnauthorizedAccessException if invalid.
    /// </summary>
    Task ValidatePasswordAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves minimal user information by ID.
    /// </summary>
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of user creation.
/// </summary>
public record CreateUserResult(Guid UserId, string Email, string FullName);

/// <summary>
/// Lightweight user data for application use (no identity internals).
/// </summary>
public record UserDto(Guid UserId, string Email, string FullName);