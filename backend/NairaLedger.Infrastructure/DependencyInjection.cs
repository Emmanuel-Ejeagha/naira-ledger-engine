namespace NairaLedger.Infrastructure;

public static class DependencyInjection
{
    private static Lazy<IConnectionMultiplexer> _redisLazy = null;

    public static IConnectionMultiplexer Connection => _redisLazy.Value;

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
        _redisLazy = new Lazy<IConnectionMultiplexer>(() =>
        {
            var redisConnection = configuration["Redis__ConnectionString"]
                   ?? configuration["Redis:ConnectionString"]
                   ?? "localhost:6379";

            if (string.IsNullOrWhiteSpace(redisConnection))
                throw new InvalidOperationException("Redis connection string is missing or invalid.");

            var options = ConfigurationOptions.Parse(redisConnection, true);

            options.AbortOnConnectFail = false;

            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 20000;
            options.SyncTimeout = 15000;
            options.AsyncTimeout = 15000;
            options.Ssl = true;
            options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                                 | System.Security.Authentication.SslProtocols.Tls13;
            options.ReconnectRetryPolicy = new LinearRetry(5000); 
            options.KeepAlive = 60;

            var multiplexer = ConnectionMultiplexer.Connect(options);

            multiplexer.ErrorMessage += (sender, args) =>
                Console.Error.WriteLine($"Redis error: {args.Message}");
            multiplexer.ConnectionFailed += (sender, args) =>
                Console.Error.WriteLine($"Redis connection failed: {args.Exception?.Message}");

            return multiplexer;
        });
        services.AddSingleton<IConnectionMultiplexer>(_ => _redisLazy.Value);

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