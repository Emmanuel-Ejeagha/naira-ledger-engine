namespace NairaLedger.WebApi.Endpoints;

public static class WebhookEndpoints
{
    public static void MapWebhookEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/webhooks/paystack", async (HttpRequest request, IPaystackService paystackService, IMediator mediator) =>
        {
            using var reader = new StreamReader(request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = request.Headers["x-paystack-signature"].FirstOrDefault() ?? "";

            var command = await paystackService.ProcessWebhookAsync(payload, signature, CancellationToken.None);
            if (command is null) return Results.BadRequest(new { error = "Invalid webhook" });

            await mediator.Send(command);
            return Results.Ok(new { status = "processed" });
        })
        .WithTags("Webhooks")
        .WithSummary("Paystack webhook receiver")
        .WithDescription("Receives and processes Paystack webhook events. Only charge.success events are handled. Signature verification is mandatory.")
        .Produces(200)
        .Produces(400);
    }
}