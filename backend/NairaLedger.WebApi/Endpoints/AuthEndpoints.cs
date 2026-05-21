using MediatR;
using NairaLedger.Application.Commands.Auth;
using NairaLedger.Application.Commands.Auth.Register;
using NairaLedger.Application.Commands.Auth.Token;

namespace NairaLedger.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireRateLimiting("strict");

        app.MapPost("/api/v1/auth/login", async (LoginUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireRateLimiting("strict");

        app.MapPost("/api/v1/auth/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).RequireRateLimiting("strict");
    }
}       