namespace NairaLedger.Application.Exceptions;

/// <summary>
/// Thrown when a user registration fails because the email is already taken.
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.") { }
}