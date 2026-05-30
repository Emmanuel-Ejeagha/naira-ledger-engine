namespace NairaLedger.Application.Commands.Auth;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;

public record LoginUserResponse(string AccessToken, string RefreshToken, string Message);