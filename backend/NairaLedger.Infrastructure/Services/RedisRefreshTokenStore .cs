namespace NairaLedger.Infrastructure.Services;

public class RedisRefreshTokenStore : IRefreshTokenStore
{
    private readonly IConnectionMultiplexer _redis;
    private const string Prefix = "refresh_token:";

    public RedisRefreshTokenStore(IConnectionMultiplexer redis) => _redis = redis;

    public async Task StoreAsync(string token, Guid userId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{Prefix}{token}";
            var entry = $"{userId}|{expiresAt:O}";

            var expiry = expiresAt - DateTime.UtcNow;
            if (expiry <= TimeSpan.Zero)
            {
                throw new ArgumentException("Token expiry must be in the future");
            }

            await db.StringSetAsync(key, entry, expiry);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Redis StoreAsync failed: {ex.Message}");
            throw; 
        }
    }

    public async Task<Guid?> ValidateAsync(string token, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var key = $"{Prefix}{token}";
        var value = await db.StringGetAsync(key);

        if (value.IsNullOrEmpty) return null;

        var parts = value.ToString().Split('|');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId))
            return null;

        return userId;
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        var key = $"{Prefix}{token}";
        await db.KeyDeleteAsync(key);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}