namespace NairaLedger.Application.Commands.FundWallet;

/// <summary>
/// Initiates a Paystack payment session via the payment gateway.
/// </summary>
public class InitiateFundingCommandHandler : IRequestHandler<InitiateFundingCommand, InitiateFundingResponse>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<InitiateFundingCommandHandler> _logger;

    public InitiateFundingCommandHandler(IPaymentGateway paymentGateway, ILogger<InitiateFundingCommandHandler> logger)
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
    }

    public async Task<InitiateFundingResponse> Handle(InitiateFundingCommand request, CancellationToken cancellationToken)
    {
        var email = "user@example.com";

        var result = await _paymentGateway.InitiatePaymentAsync(request.Amount, email, request.CallbackUrl, request.WalletId, cancellationToken);
        return new InitiateFundingResponse(result.AuthorizationUrl, result.Reference);
    }
}