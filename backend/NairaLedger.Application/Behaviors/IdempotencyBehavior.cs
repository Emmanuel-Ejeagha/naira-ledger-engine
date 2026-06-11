namespace NairaLedger.Application.Behaviors;

/// <summary>
/// Ensures idempotency for commands implementing IIdempotentCommand.
/// If the key has been processed, returns the previously stored result.
/// </summary>
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
{
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(IIdempotencyStore idempotencyStore, ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _idempotencyStore = idempotencyStore;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var key = request.IdempotencyKey;
        var existing = await _idempotencyStore.GetResponseAsync(key, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Idempotency key {Key} already processed. Returning stored result.", key);
            if (existing.Result is TResponse response)
                return response;
            throw new InvalidOperationException($"Stored response type mismatch for idempotency key {key}.");
        }

        var result = await next();

        await _idempotencyStore.StoreResponseAsync(key, new IdempotentResponse(result, null), cancellationToken);
        return result;
    }
}