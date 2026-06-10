using StackExchange.Redis;

namespace NairaLedger.WebApi.Endpoints
{
    public static class RedisEndpoint
    {
        public static void MapRedisEndpoints(this WebApplication app)
        {
            app.MapGet("/health/redis", async (IConnectionMultiplexer redis) =>
            {
                var db = redis.GetDatabase();
                var pong = await db.PingAsync();
                return Results.Ok($"Redis status: {pong}");
            });

        }
    }
}
