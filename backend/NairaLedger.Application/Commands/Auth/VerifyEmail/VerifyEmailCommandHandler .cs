using MediatR;
using NairaLedger.Application.Commands.Auth;
using NairaLedger.Application.Interfaces;

namespace NairaLedger.Application.Commands.Auth;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{
    private readonly IUserService _userService;

    public VerifyEmailCommandHandler(IUserService userService) => _userService = userService;

    public async Task<VerifyEmailResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        await _userService.VerifyEmailAsync(request.Email, request.Token, cancellationToken);
        return new VerifyEmailResponse("Email verified successfully.");
    }
}