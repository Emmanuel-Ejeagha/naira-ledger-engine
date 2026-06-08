using MediatR;

namespace NairaLedger.Application.Commands.Admin;

public record CreateAdminUserCommand(string Email, string FullName, string Password) : IRequest<CreateAdminUserResponse>;

public record CreateAdminUserResponse(Guid UserId, string Message);