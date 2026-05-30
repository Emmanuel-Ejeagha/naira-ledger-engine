namespace NairaLedger.Application.Commands.Auth;

/// <summary>
/// Verifies a user's email address using the token sent after registration.
/// </summary>
public record VerifyEmailCommand(string Email, string Token) : IRequest<VerifyEmailResponse>;

public record VerifyEmailResponse(string Message);