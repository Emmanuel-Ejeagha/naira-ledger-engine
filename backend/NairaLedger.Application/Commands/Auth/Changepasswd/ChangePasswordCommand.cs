namespace NairaLedger.Application.Commands.Auth;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;