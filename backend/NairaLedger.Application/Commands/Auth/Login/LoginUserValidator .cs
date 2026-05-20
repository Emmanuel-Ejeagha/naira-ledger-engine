using FluentValidation;
using NairaLedger.Application.Commands.Auth;

namespace NairaLedger.Application.Commands.Auth;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}