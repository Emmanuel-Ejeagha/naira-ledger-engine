namespace NairaLedger.WebApi.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/transactions", [Authorize] async (Guid walletId, string? cursor, int pageSize, IMediator mediator) =>
        {
            var query = new GetTransactionHistoryQuery(walletId, cursor, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithTags("Transactions")
        .WithSummary("Transaction history")
        .WithDescription("Returns a cursor‑paginated list of transactions for the specified wallet. Default page size is 20.")
        .Produces<PagedResponse<TransactionDto>>(200)
        .RequireRateLimiting("relaxed");
    }
}