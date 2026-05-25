namespace NairaLedger.Application.Commands.Auth.Token;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;