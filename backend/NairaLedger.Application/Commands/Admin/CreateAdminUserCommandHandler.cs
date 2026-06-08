using MediatR;
using NairaLedger.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace NairaLedger.Application.Commands.Admin;

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, CreateAdminUserResponse>
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateAdminUserCommandHandler> _logger;

    public CreateAdminUserCommandHandler(IUserService userService, ILogger<CreateAdminUserCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<CreateAdminUserResponse> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUserAsync(request.Email, request.FullName, request.Password, cancellationToken);
        await _userService.AddToRoleAsync(result.UserId, "Admin", cancellationToken);
        _logger.LogInformation("Admin user created: {Email}", request.Email);
        return new CreateAdminUserResponse(result.UserId, "Admin user created successfully.");
    }
}