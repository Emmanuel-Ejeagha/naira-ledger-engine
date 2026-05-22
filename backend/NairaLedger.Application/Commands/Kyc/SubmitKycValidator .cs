using FluentValidation;

namespace NairaLedger.Application.Commands.Kyc;

public class SubmitKycValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycValidator()
    {
        RuleFor(x => x.WalletId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.IdNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IdType).NotEmpty();
    }
}