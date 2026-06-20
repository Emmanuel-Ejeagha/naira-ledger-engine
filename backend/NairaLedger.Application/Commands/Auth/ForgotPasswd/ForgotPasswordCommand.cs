namespace NairaLedger.Application.Commands.Auth.ForgotPasswd;

public record ForgotPasswordCommand(string Email) : IRequest<Unit>;

