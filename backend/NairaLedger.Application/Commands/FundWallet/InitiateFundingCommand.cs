using MediatR;

namespace NairaLedger.Application.Commands.FundWallet;

/// <summary>
/// Initiates a Paystack payment session and returns a payment authorization URL.
/// </summary>
public record InitiateFundingCommand(Guid WalletId, decimal Amount, string CallbackUrl) : IRequest<InitiateFundingResponse>;

public record InitiateFundingResponse(string AuthorizationUrl, string Reference);