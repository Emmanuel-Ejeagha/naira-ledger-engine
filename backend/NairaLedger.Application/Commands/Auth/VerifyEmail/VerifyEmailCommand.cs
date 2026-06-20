namespace NairaLedger.Application.Commands.Auth.VerifyEmail;

/// <summary>
/// Verifies a user's email address using the token sent after registration.
/// </summary>
public record VerifyEmailCommand(Guid UserId, string Token) : IRequest<Unit>;
