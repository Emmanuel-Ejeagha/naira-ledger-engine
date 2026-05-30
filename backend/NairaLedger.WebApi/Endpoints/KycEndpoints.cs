namespace NairaLedger.WebApi.Endpoints;

public static class KycEndpoints
{
    public static void MapKycEndpoints(this WebApplication app)
    {
        var kycGroup = app.MapGroup("/api/v1/kyc")
            .WithTags("KYC");

        kycGroup.MapPost("/submit", [Authorize] async (SubmitKycCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "KYC submitted" });
        })
        .WithSummary("Submit KYC documents")
        .WithDescription("Submits KYC information for the given wallet. The wallet KYC level will be upgraded to Tier1 upon submission.")
        .Produces(200)
        .ProducesProblem(400);

        kycGroup.MapPost("/approve", [Authorize(Roles = "Admin")] async (ApproveKycCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "KYC approved" });
        })
        .WithSummary("Approve KYC (Admin)")
        .WithDescription("Admin endpoint to upgrade a wallet's KYC level. Level can only be increased.")
        .Produces(200)
        .ProducesProblem(403)
        .RequireRateLimiting("strict");
    }
}