namespace NairaLedger.Application.Commands.Auth.ResetPasswd;

public record ResetPasswordCommand(Guid UserId, string Token, string NewPassword, string Email) : IRequest<Unit>;