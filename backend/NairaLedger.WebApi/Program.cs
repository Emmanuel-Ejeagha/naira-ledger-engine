using NairaLedger.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ──────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console());

// ── Infrastructure & Application ────────────────────
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// ── API Explorer & Swagger ──────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NairaLedger API",
        Version = "v1",
        Description = "Production-grade Digital Wallet & Double-Entry Ledger"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token WITHOUT the 'Bearer' prefix"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

// ── Authentication & Authorization ──────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

builder.Services.AddAuthorization(options =>
{
    Policies.Configure(options);
});

// ── SignalR ─────────────────────────────────────────
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealTimeNotifier, SignalRRealTimeNotifier>();

// ── Rate Limiting ───────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("strict", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
    });
    options.AddFixedWindowLimiter("moderate", config =>
    {
        config.PermitLimit = 20;
        config.Window = TimeSpan.FromMinutes(1);
    });
    options.AddFixedWindowLimiter("relaxed", config =>
    {
        config.PermitLimit = 100;
        config.Window = TimeSpan.FromMinutes(1);
    });
});

// ── Health Checks ───────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "PostgreSQL")
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!, name: "Redis");

// ── Hangfire ────────────────────────────────────────
builder.Services.AddHangfireServer();

var app = builder.Build();

// ── Middleware Pipeline ─────────────────────────────
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NairaLedgerEngine API v1");
});

app.UseHttpsRedirection();

app.UseForwardedHeaders();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ───────────────────────────────────────
app.MapAuthEndpoints();
app.MapWalletEndpoints();
app.MapKycEndpoints();
app.MapTransferEndpoints();
app.MapTransactionEndpoints();
app.MapWebhookEndpoints();

// ── SignalR Hub ─────────────────────────────────────
app.MapHub<NotificationHub>("/hubs/notifications");

// ── Health Checks ───────────────────────────────────
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// ── Hangfire Dashboard ──────────────────────────────
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// ── Migrate & Seed ──────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NairaLedger.Infrastructure.Persistence.NairaLedgerDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// ── Recurring Jobs ──────────────────────────────────
try
{
    RecurringJob.AddOrUpdate<OutboxPublisherJob>(
        "outbox-publisher",
        job => job.ExecuteAsync(),
        Cron.Minutely);
}
catch (Exception ex)
{
    // Log and continue – the job will be added on next restart or manually
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Could not register recurring job. It may already be present.");
}
await app.RunAsync();

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}