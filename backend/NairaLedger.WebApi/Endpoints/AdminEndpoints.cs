using NairaLedger.Infrastructure.Persistence;
using NairaLedger.Domain.Enums;
using NairaLedger.Application.Commands.Admin;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Interfaces;

namespace NairaLedger.WebApi.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var adminGroup = app.MapGroup("/api/v1/admin")
            .RequireAuthorization("Admin")
            .WithTags("Admin");

        // Get wallets with pending KYC (Tier1)
        adminGroup.MapGet("/kyc/pending", async (NairaLedgerDbContext db) =>
        {
            var wallets = await db.Wallets
                .Where(w => w.KycLevel == KycLevel.Tier1)
                .Select(w => new {
                    w.Id,
                    w.UserId.Value,
                    Tag = w.Tag != null ? w.Tag.Value : null,
                    w.KycLevel,
                    w.CreatedAt,
                    w.KycFullName,
                    w.KycIdNumber,
                    w.KycIdType
                })
                .ToListAsync();
            return Results.Ok(wallets);
        })
        .WithSummary("Get wallets with pending KYC (Tier1)");

        // Approve KYC (with email)
        adminGroup.MapPost("/kyc/approve/{walletId:guid}", async (Guid walletId, NairaLedgerDbContext db, IEmailService emailService, IWalletRepository walletRepo) =>
        {
            var wallet = await db.Wallets.FindAsync(walletId);
            if (wallet is null) return Results.NotFound();
            wallet.VerifyKyc(KycLevel.Tier2);
            await db.SaveChangesAsync();

            try
            {
                var ownerInfo = await walletRepo.GetOwnerInfoAsync(walletId);
                if (ownerInfo is not null)
                    await emailService.SendKycApprovedEmailAsync(ownerInfo.Email, ownerInfo.FullName, CancellationToken.None);
            }
            catch { /* ignore email errors */ }

            return Results.Ok(new { message = "KYC approved" });
        })
        .WithSummary("Approve KYC (upgrade to Tier2)");

        // Reject KYC (with email)
        adminGroup.MapPost("/kyc/reject/{walletId:guid}", async (Guid walletId, NairaLedgerDbContext db, IEmailService emailService, IWalletRepository walletRepo) =>
        {
            var wallet = await db.Wallets.FindAsync(walletId);
            if (wallet is null) return Results.NotFound();
            wallet.RejectKyc();
            await db.SaveChangesAsync();

            try
            {
                var ownerInfo = await walletRepo.GetOwnerInfoAsync(walletId);
                if (ownerInfo is not null)
                    await emailService.SendKycRejectedEmailAsync(ownerInfo.Email, ownerInfo.FullName, "KYC documents not satisfactory.", CancellationToken.None);
            }
            catch { }

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

        // Create admin user
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