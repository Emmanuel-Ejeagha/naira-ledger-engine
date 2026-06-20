namespace NairaLedger.Application.Commands.Auth.VerifyEmail;

public record SendVerificationEmailCommand(string Email) : IRequest<Unit>;
