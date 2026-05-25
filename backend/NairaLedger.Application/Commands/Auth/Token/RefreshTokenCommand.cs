namespace NairaLedger.Application.Commands.Auth.Token;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<LoginUserResponse>;