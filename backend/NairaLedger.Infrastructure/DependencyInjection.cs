using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.Interfaces;
using NairaLedger.Infrastructure.Identity;
using NairaLedger.Infrastructure.Outbox;
using NairaLedger.Infrastructure.Persistence;
using NairaLedger.Infrastructure.Persistence.Repositories;
using NairaLedger.Infrastructure.Services;
using StackExchange.Redis;

namespace NairaLedger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Database
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Core services
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddScoped<ILedgerQueryService, LedgerQueryService>();
        services.AddScoped<ITransactionQueryService, TransactionQueryService>();
        services.AddScoped<IFraudEscalationService, FraudDetectionService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserWalletResolver, WalletRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Settings
        services.Configure<PaystackSettings>(configuration.GetSection("Paystack"));

        // Paystack webhook service (HttpClient)
        services.AddHttpClient<IPaystackService, PaystackService>(client =>
        {
            client.BaseAddress = new Uri("https://api.paystack.co");
        });

        // Paystack payment gateway (HttpClient)
        services.AddHttpClient<IPaymentGateway, PaystackPaymentGateway>();

        // JWT & refresh tokens
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenStore, RedisRefreshTokenStore>();

        // Outbox publisher
        services.AddScoped<OutboxPublisherJob>();

        // SMTP
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

        return services;
    }
}