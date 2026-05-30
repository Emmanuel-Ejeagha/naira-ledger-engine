namespace NairaLedger.Infrastructure.Services;

public class IdempotencyStore : IIdempotencyStore
{
    private readonly NairaLedgerDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    private const string Prefix = "idem:";

    public IdempotencyStore(NairaLedgerDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _redis = redis;
    }

    public async Task<IdempotentResponse?> GetResponseAsync(IdempotencyKey key, CancellationToken cancellationToken)
    {
        // Check Redis first
        var db = _redis.GetDatabase();
        var redisKey = $"{Prefix}{key.Value}";
        var cached = await db.StringGetAsync(redisKey);
        if (!cached.IsNullOrEmpty)
        {
            var json = cached.ToString();
            return JsonSerializer.Deserialize<IdempotentResponse>(json);
        }

        // Fallback to DB
        var record = await _context.IdempotencyRecords.FindAsync(new object[] { key.Value }, cancellationToken);
        if (record is null) return null;

        var deserialized = JsonSerializer.Deserialize<IdempotentResponse>(record.ResponseData);
        await db.StringSetAsync(redisKey, record.ResponseData, TimeSpan.FromHours(24));
        return deserialized;
    }

    public async Task StoreResponseAsync(IdempotencyKey key, IdempotentResponse response, CancellationToken cancellationToken)
    {
        var record = new IdempotencyRecord
        {
            Key = key.Value,
            ResponseData = JsonSerializer.Serialize(response)
        };
        _context.IdempotencyRecords.Add(record);

        var db = _redis.GetDatabase();
        var redisKey = $"{Prefix}{key.Value}";
        await db.StringSetAsync(redisKey, record.ResponseData, TimeSpan.FromHours(24));
    }
}