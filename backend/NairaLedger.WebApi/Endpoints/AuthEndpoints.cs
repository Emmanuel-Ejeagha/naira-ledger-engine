namespace NairaLedger.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        authGroup.MapPost("/register", async (RegisterUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account and automatically provisions a wallet. Returns user ID and wallet ID.")
        .Produces<RegisterUserResponse>(200)
        .ProducesProblem(400)
        .RequireRateLimiting("strict");

        authGroup.MapPost("/login", async (LoginUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("User login")
        .WithDescription("Authenticates a user and returns a JWT access token and a refresh token.")
        .Produces<LoginUserResponse>(200)
        .ProducesProblem(401)
        .RequireRateLimiting("strict");

        authGroup.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithSummary("Refresh access token")
        .WithDescription("Uses a valid refresh token to obtain a new access token. The old refresh token is revoked.")
        .Produces<LoginUserResponse>(200)
        .ProducesProblem(401)
        .RequireRateLimiting("strict");
    }
}