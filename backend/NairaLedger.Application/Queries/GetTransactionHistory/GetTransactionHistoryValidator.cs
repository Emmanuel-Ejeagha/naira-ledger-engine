using FluentValidation;

namespace NairaWallet.Application.Queries.GetTransactionHistory;

/// <summary>
/// Validates pagination and filter parameters for transaction history.
/// </summary>
public class GetTransactionHistoryValidator : AbstractValidator<GetTransactionHistoryQuery>
{
    public GetTransactionHistoryValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Wallet ID is required.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}