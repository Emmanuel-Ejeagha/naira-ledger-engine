namespace NairaLedger.Application.Commands.CreateWallet;

public class CreateWalletValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(userId => userId.Value != Guid.Empty).WithMessage("UserId must be a valid GUID.");
        RuleFor(x => x.Tag)
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.");
    }
}
