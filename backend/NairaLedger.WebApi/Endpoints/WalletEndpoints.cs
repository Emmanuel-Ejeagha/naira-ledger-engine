using MediatR;
using Microsoft.AspNetCore.Authorization;
using NairaLedger.Application.Commands.CreateWallet;
using NairaLedger.Application.Queries.GetWalletBalance;

namespace NairaLedger.WebApi.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/wallets", [Authorize] async (CreateWalletCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        app.MapGet("/api/v1/wallets/{walletId:guid}/balance", [Authorize] async (Guid walletId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetWalletBalanceQuery(walletId));
            return Results.Ok(result);
        });
    }
}