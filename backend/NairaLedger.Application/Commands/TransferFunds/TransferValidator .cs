using FluentValidation;

namespace NairaWallet.Application.Commands.TransferFunds;

/// <summary>
/// Validates the TransferCommand.
/// </summary>
public class TransferValidator : AbstractValidator<TransferCommand>
{
    public TransferValidator()
    {
        RuleFor(x => x.FromWalletId)
            .NotEmpty().WithMessage("Sender wallet ID is required.")
            .NotEqual(x => x.ToWalletId).WithMessage("Cannot transfer to the same wallet.");

        RuleFor(x => x.ToWalletId)
            .NotEmpty().WithMessage("Recipient wallet ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be greater than zero.");

        RuleFor(x => x.IdempotencyKey.Value)
            .NotEmpty().WithMessage("Idempotency key is required.");
    }
}