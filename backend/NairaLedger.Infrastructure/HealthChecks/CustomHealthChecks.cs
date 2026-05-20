using Microsoft.Extensions.DependencyInjection;

namespace NairaLedger.Infrastructure.HealthChecks;

public static class CustomHealthChecks
{
    public static void AddNairaLedgerHealthChecks(this IServiceCollection services, string connectionString, string redisConnection)
    {
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "PostgreSQL")
            .AddRedis(redisConnection, name: "Redis")
            .AddHangfire(options => { options.MinimumAvailableServers = 1; }, name: "Hangfire");
    }
}