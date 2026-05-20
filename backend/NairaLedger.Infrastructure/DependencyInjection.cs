using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Infrastructure.HealthChecks;
using NairaLedger.Infrastructure.Identity;
using NairaLedger.Infrastructure.Outbox;
using NairaLedger.Infrastructure.Persistence;
using NairaLedger.Infrastructure.Persistence.Repositories;
using NairaLedger.Infrastructure.Services;
using NairaWallet.Application.Interfaces;
using StackExchange.Redis;

namespace NairaLedger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<NairaLedgerDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<NairaLedgerDbContext>()
        .AddDefaultTokenProviders();

        // Redis
        var redisConnection = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

        // Hangfire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(connectionString));
        services.AddHangfireServer();

        // Repositories
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddScoped<ILedgerQueryService, LedgerQueryService>();
        services.AddScoped<ITransactionQueryService, TransactionQueryService>();
        services.AddScoped<IFraudEscalationService, FraudDetectionService>();
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaystackService>(sp =>
        {
            var secretKey = configuration["Paystack:SecretKey"] ?? "";
            var resolver = sp.GetRequiredService<IUserWalletResolver>();
            var logger = sp.GetRequiredService<ILogger<PaystackService>>();
            return new PaystackService(secretKey, resolver, logger);
        });
        services.AddNairaLedgerHealthChecks(connectionString, redisConnection);
        // JWT
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenStore, RedisRefreshTokenStore>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddScoped<IUserService, UserService>();






        // Outbox job
        services.AddScoped<OutboxPublisherJob>();

        services.AddHealthChecks()
           .AddNpgSql(connectionString, name: "PostgreSQL")
           .AddRedis(redisConnection, name: "Redis");

        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));


        return services;
    }
}