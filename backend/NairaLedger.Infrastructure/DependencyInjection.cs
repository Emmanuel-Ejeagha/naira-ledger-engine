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
using System.Net.Http.Headers;   // for AuthenticationHeaderValue

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
            options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
        })
        .AddEntityFrameworkStores<NairaLedgerDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("EmailConfirmationTokenProvider");

        services.AddTransient<EmailConfirmationTokenProvider<AppUser>>();
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(2);
        });

        // Redis
        var env = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
        var redisConnection = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        if (env == "Development")
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
        }
        else
        {
            var lazyRedis = new Lazy<IConnectionMultiplexer>(() =>
            {
                if (string.IsNullOrWhiteSpace(redisConnection))
                    throw new InvalidOperationException("Redis connection string is missing or invalid.");

                var uri = new Uri(redisConnection);
                var password = uri.UserInfo.Split(':').LastOrDefault() ?? "";
                var host = uri.Host;
                var port = uri.Port;

                var options = new ConfigurationOptions
                {
                    EndPoints = { { host, port } },
                    Password = password,
                    Ssl = true,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                             | System.Security.Authentication.SslProtocols.Tls13,
                    AbortOnConnectFail = false,
                    ConnectTimeout = 20000,
                    SyncTimeout = 15000,
                    AsyncTimeout = 15000,
                    KeepAlive = 60,
                    ReconnectRetryPolicy = new LinearRetry(5000)
                };

                var multiplexer = ConnectionMultiplexer.Connect(options);

                multiplexer.ErrorMessage += (sender, args) =>
                    Console.Error.WriteLine($"Redis error: {args.Message}");
                multiplexer.ConnectionFailed += (sender, args) =>
                    Console.Error.WriteLine($"Redis connection failed: {args.Exception?.Message}");

                Console.WriteLine($"Redis connected: {multiplexer.IsConnected} to {host}:{port}");
                return multiplexer;
            });

            services.AddSingleton<IConnectionMultiplexer>(_ => lazyRedis.Value);
        }   // ← this closing brace was missing

        // Hangfire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(
                    connectionString,
                    new PostgreSqlStorageOptions
                    {
                        DistributedLockTimeout = TimeSpan.FromSeconds(30),
                        QueuePollInterval = TimeSpan.FromSeconds(15)
                    }));
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

        // Email – SendGrid API (replaces SMTP)
        //services.AddHttpClient<IEmailService, SendGridApiEmailService>((sp, client) =>
        //{
        //    client.BaseAddress = new Uri("https://api.sendgrid.com/v3/");
        //    var smtpOptions = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
        //    client.DefaultRequestHeaders.Authorization =
        //        new AuthenticationHeaderValue("Bearer", smtpOptions.Password);
        //});

        //services.AddScoped<IUserWalletResolver, WalletRepository>();
        //services.AddScoped<IUserService, UserService>();
        //services.AddScoped<INotificationService, NotificationService>();
        //services.AddSingleton<IEmailTemplateService, EmailTemplateService>();

        services.AddScoped<IEmailService, EmailService>();

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

        // SMTP (SendGrid API doesn't need SmtpSettings, but keep for backward compatibility)
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddMemoryCache();

        return services;
    }
}