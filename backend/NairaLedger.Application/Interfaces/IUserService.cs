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

    Task VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of role names for the given user.
    /// </summary>
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    /// <summary>
    /// Generates an email confirmation token for the user, which can be sent in a verification email. The token is typically a secure string that encodes the user's identity and expiration time.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the user's email using the provided token. This method validates the token and, if valid, marks the user's email as confirmed in the system. If the token is invalid or expired, it should throw an appropriate exception.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of user creation.
/// </summary>
public record CreateUserResult(Guid UserId, string Email, string FullName);

