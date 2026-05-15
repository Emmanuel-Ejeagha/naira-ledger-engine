using FluentValidation;

namespace NairaWallet.Application.Commands.FundWallet;

public class FundWalletValidator : AbstractValidator<FundWalletCommand>
{
    public FundWalletValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Wallet ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.IdempotencyKey.Value)
            .NotEmpty().WithMessage("Idempotency key is required.")
            .MaximumLength(128).WithMessage("Idempotency key must not exceed 128 characters.");
    }
}