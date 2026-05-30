namespace NairaLedger.WebApi.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this WebApplication app)
    {
        var transferGroup = app.MapGroup("/api/v1/transfers")
            .WithTags("Transfers")
            .RequireAuthorization();

        transferGroup.MapPost(string.Empty, async (TransferCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("P2P transfer")
        .WithDescription("Transfers NGN from one wallet to another. Requires a unique idempotency key. Balance is debited immediately.")
        .Produces<TransferResponse>(200)
        .ProducesProblem(400)
        .RequireRateLimiting("moderate");

        transferGroup.MapPost("/{transactionId:guid}/reverse", [Authorize(Roles = "Admin")] async (Guid transactionId, IMediator mediator) =>
        {
            var command = new ReverseTransactionCommand(transactionId, null);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Reverse a transaction (Admin)")
        .WithDescription("Reverses a completed transaction within the 30‑minute reversal window. Only allowed for administrators.")
        .Produces<ReverseTransactionResponse>(200)
        .ProducesProblem(403)
        .RequireRateLimiting("strict");
    }
}