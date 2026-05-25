namespace NairaLedger.Application.Commands.FundWallet;

public class InitiateFundingValidator : AbstractValidator<InitiateFundingCommand>
{
    public InitiateFundingValidator()
    {
        RuleFor(x => x.WalletId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CallbackUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _));
    }
}