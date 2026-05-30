namespace NairaLedger.Application.Commands.CreateWallet;

/// <summary>
/// Creates a new wallet for an existing user.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Tag">An optional tag for the wallet.</param>
public record CreateWalletCommand(UserId UserId, string? Tag) : IRequest<CreateWalletResponse>;

/// <summary>
/// Result of wallet creation, containing the new wallet's unique identifier and a message indicating success or failure.
/// </summary>
/// <param name="WalletId">The unique identifier of the newly created wallet.</param>
/// <param name="Message">A message indicating the result of the wallet creation.   </param>
public record CreateWalletResponse(Guid WalletId, string Message);
