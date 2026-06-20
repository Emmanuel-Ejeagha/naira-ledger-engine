using Microsoft.Extensions.Caching.Memory;
using NairaLedger.Application.Commands.Auth.VerifyEmail;

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

        authGroup.MapPost("/change-password", [Authorize] async (ChangePasswordCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Password changed successfully." });
        })
        .WithSummary("Change password")
        .WithDescription("Changes the password for the currently authenticated user.")
        .Produces(200)
        .ProducesProblem(400);

        authGroup.MapPost("/send-verification-email", async (SendVerificationEmailCommand command, IMediator mediator, HttpContext httpcontext) =>
        {
            var emailKey = $"verify-resend:{command.Email}";
            var cache = httpcontext.RequestServices.GetRequiredService<IMemoryCache>();
            if (cache.TryGetValue(emailKey, out _))
                return Results.Problem("Verification email already sent. Please wait before trying again.", statusCode: 429);
            
            await mediator.Send(command);
            cache.Set(emailKey, true, TimeSpan.FromSeconds(60));
            return Results.Ok(new { message = "Verification email sent." });
        })
        .WithSummary("Send verification email")
        .WithDescription("Sends a verification email to the specified email address.")
        .Produces(200)
        .ProducesProblem(400);

        authGroup.MapGet("/verify-email", async (Guid userId, string token, IMediator mediator) =>
        {
            await mediator.Send(new VerifyEmailCommand(userId, token));
            return Results.Ok(new { message = "Email verified successfully." });
        })
        .WithSummary("Verify email address")
        .WithDescription("Sends a verification email to the specified email address. Rate‑limited to 1 request per 60 seconds.")
        .Produces(200)
        .ProducesProblem(400)
        .ProducesProblem(429);
    }
}