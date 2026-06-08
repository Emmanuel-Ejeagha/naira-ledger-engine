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


        app.MapGet("/api/v1/transactions/export/pdf", [Authorize] async (Guid walletId, IMediator mediator) =>
        {
            // TODO: Implement real PDF generation with QuestPDF
            var bytes = Encoding.UTF8.GetBytes("PDF export placeholder");
            return Results.File(bytes, "application/pdf", "statement.pdf");
        })
        .WithSummary("Export transactions as PDF")
        .WithDescription("Downloads a PDF statement for the given wallet.");

        app.MapGet("/api/v1/transactions/export/csv", [Authorize] async (Guid walletId, IMediator mediator) =>
        {
            // TODO: Implement server‑side CSV export
            var csv = "Reference,Type,Amount,Status,Date\n";
            return Results.File(Encoding.UTF8.GetBytes(csv), "text/csv", "transactions.csv");
        });
    }
}