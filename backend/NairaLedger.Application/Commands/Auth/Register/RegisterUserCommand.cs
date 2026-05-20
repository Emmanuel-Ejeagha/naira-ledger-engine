using MediatR;

namespace NairaLedger.Application.Commands.Auth.Register;

/// <summary>
/// Registers a new user and creates a wallet.
/// </summary>
public record RegisterUserCommand(string Email, string FullName, string Password) : IRequest<RegisterUserResponse>;

public record RegisterUserResponse(Guid UserId, Guid WalletId, string Message);