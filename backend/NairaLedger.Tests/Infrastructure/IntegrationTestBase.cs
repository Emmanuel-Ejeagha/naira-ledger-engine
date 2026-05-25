

namespace NairaLedger.Tests.Infrastructure;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected PostgreSqlContainer PostgresContainer { get; private set; } = null!;
    protected RedisContainer RedisContainer { get; private set; } = null!;
    protected IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        PostgresContainer = new PostgreSqlBuilder()
            .WithDatabase("naira_ledger_test")
            .WithUsername("test_user")
            .WithPassword("test_pass")
            .Build();

        RedisContainer = new RedisBuilder()
            .Build();

        await PostgresContainer.StartAsync();
        await RedisContainer.StartAsync();

        var services = new ServiceCollection();

        // Fake configuration
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = PostgresContainer.GetConnectionString(),
                ["Redis:ConnectionString"] = RedisContainer.GetConnectionString(),
                ["Jwt:Secret"] = "AJnkfbcuuiJFjZRJkfnioNikloNknIKNDIOAkmnfnblnlnlnfaE",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:AccessTokenExpirationMinutes"] = "15",
                ["Paystack:SecretKey"] = "test_secret",
                ["Smtp:Host"] = "localhost",
                ["Smtp:Port"] = "1025",
                ["Smtp:Username"] = "",
                ["Smtp:Password"] = "",
                ["Smtp:FromAddress"] = "no-reply@test.com",
                ["Smtp:FromName"] = "Test"
            });

        var configuration = configBuilder.Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Add application & infrastructure
        services.AddApplication();       
        services.AddInfrastructure(configuration);

        // Add logging
        services.AddLogging();

        ServiceProvider = services.BuildServiceProvider();

        // Apply migrations
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NairaLedgerDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (PostgresContainer is not null)
            await PostgresContainer.DisposeAsync();
        if (RedisContainer is not null)
            await RedisContainer.DisposeAsync();
    }
}