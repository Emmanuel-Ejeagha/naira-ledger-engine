using NairaLedger.Domain.ValueObjects;
using System.Security.Claims;

namespace NairaLedger.WebApi.Endpoints;

public static class WalletEndpoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        var walletGroup = app.MapGroup("/api/v1/wallets")
            .WithTags("Wallets")
            .RequireAuthorization();

        walletGroup.MapPost(string.Empty, async (CreateWalletCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Create wallet")
        .WithDescription("Creates a new wallet for the authenticated user. Only one wallet per user is allowed.")
        .Produces<CreateWalletResponse>(200)
        .ProducesProblem(400);

        walletGroup.MapGet("/{walletId:guid}", async (Guid walletId, IWalletRepository walletRepo) =>
        {
            var wallet = await walletRepo.GetByIdAsync(walletId);
            if (wallet is null) return Results.NotFound();
            return Results.Ok(new
            {
                wallet.Id,
                wallet.UserId,
                wallet.Tag,
                wallet.KycLevel,
                wallet.IsActive,
                wallet.CreatedAt
            });
        })
        .WithSummary("Get wallet details")
        .WithDescription("Returns the wallet details for the given wallet ID.")
        .Produces(200)
        .Produces(404);

        walletGroup.MapGet("/me", async (
            IWalletRepository walletRepo,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var wallet = await walletRepo.GetByUserIdAsync(new UserId(userId));
                    if (wallet is null)
                        return Results.NotFound(new { error = "No wallet found. Please create one first." });

                    return Results.Ok(new
                    {
                        wallet.Id,
                        wallet.UserId,
                        wallet.Tag,
                        wallet.KycLevel,
                        wallet.IsActive,
                        wallet.CreatedAt
                    });
                })
        .WithSummary("Get current user's wallet")
        .WithDescription("Returns the wallet belonging to the authenticated user.")
        .Produces(200)
        .Produces(401)
        .RequireAuthorization();

        walletGroup.MapGet("/{walletId:guid}/balance", async (Guid walletId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetWalletBalanceQuery(walletId));
            return Results.Ok(result);
        })
        .WithSummary("Get wallet balance")
        .WithDescription("Returns the current NGN balance of the specified wallet, computed from the ledger.")
        .Produces<WalletBalanceDto>(200)
        .ProducesProblem(404);

        walletGroup.MapPost("/{walletId:guid}/fund", async (Guid walletId, InitiateFundingCommand command, IMediator mediator) =>
        {
            if (command.WalletId != walletId) return Results.BadRequest("Wallet ID mismatch.");
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Initiate Paystack funding")
        .WithDescription("Starts a Paystack payment session for the given amount and returns an authorization URL. The user must complete the payment on Paystack.")
        .Produces<InitiateFundingResponse>(200)
        .ProducesProblem(400)
        .RequireRateLimiting("moderate");

    }
}