using NairaLedger.Infrastructure.Persistence;
using NairaLedger.Domain.Enums;
using NairaLedger.Application.Commands.Admin;

namespace NairaLedger.WebApi.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var adminGroup = app.MapGroup("/api/v1/admin")
            .RequireAuthorization("Admin")   // policy name
            .WithTags("Admin");

        // Get wallets with pending KYC (Tier1)
        adminGroup.MapGet("/kyc/pending", async (NairaLedgerDbContext db) =>
        {
            var wallets = await db.Wallets
                .Where(w => w.KycLevel == KycLevel.Tier1)
                .Select(w => new {
                    w.Id,
                    w.UserId,
                    Tag = w.Tag != null ? w.Tag.Value : null,
                    w.KycLevel,
                    w.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(wallets);
        })
        .WithSummary("Get wallets with pending KYC (Tier1)");

        // Approve KYC
        adminGroup.MapPost("/kyc/approve/{walletId:guid}", async (Guid walletId, NairaLedgerDbContext db) =>
        {
            var wallet = await db.Wallets.FindAsync(walletId);
            if (wallet is null) return Results.NotFound();
            wallet.VerifyKyc(KycLevel.Tier2);   // upgrade to Tier2
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "KYC approved" });
        })
        .WithSummary("Approve KYC (upgrade to Tier2)");

        // Reject KYC (set back to Unverified)
        adminGroup.MapPost("/kyc/reject/{walletId:guid}", async (Guid walletId, NairaLedgerDbContext db) =>
        {
            var wallet = await db.Wallets.FindAsync(walletId);
            if (wallet is null) return Results.NotFound();
            wallet.RejectKyc();   // method we'll add below
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "KYC rejected" });
        })
        .WithSummary("Reject KYC (set to Unverified)");

        // Reverse a transaction
        adminGroup.MapPost("/transactions/{transactionId:guid}/reverse", async (Guid transactionId, IMediator mediator) =>
        {
            var command = new ReverseTransactionCommand(transactionId, null);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Reverse a transaction (Admin)");

        // Inside MapAdminEndpoints, add:
        adminGroup.MapPost("/users/create-admin", async (CreateAdminUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Create a new admin user (Admin only)")
        .Produces(200)
        .Produces(400);
    }
}