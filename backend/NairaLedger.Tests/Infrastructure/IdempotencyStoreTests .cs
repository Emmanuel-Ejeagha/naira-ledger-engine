using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Application.Interfaces;
using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Tests.Infrastructure;

public class IdempotencyStoreTests : IntegrationTestBase
{
    private IIdempotencyStore Store => ServiceProvider.GetRequiredService<IIdempotencyStore>();
    private IUnitOfWork UnitOfWork => ServiceProvider.GetRequiredService<IUnitOfWork>();

    [Fact]
    public async Task StoreAndRetrieve_ShouldReturnSameResponse()
    {
        var key = new IdempotencyKey(Guid.NewGuid().ToString());
        var response = new IdempotentResponse(new { result = "ok" }, null);

        await Store.StoreResponseAsync(key, response, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync(); // persist

        var retrieved = await Store.GetResponseAsync(key, CancellationToken.None);
        retrieved.Should().NotBeNull();
        retrieved!.Result.Should().BeEquivalentTo(new { result = "ok" });
    }

    [Fact]
    public async Task DuplicateKey_ShouldReturnFirstResponse()
    {
        var key = new IdempotencyKey("dup-key-2");
        var first = new IdempotentResponse("first", null);
        var second = new IdempotentResponse("second", null);

        await Store.StoreResponseAsync(key, first, CancellationToken.None);
        await UnitOfWork.SaveChangesAsync();

       
        var retrieved = await Store.GetResponseAsync(key, CancellationToken.None);
        retrieved!.Result.Should().Be("first");

        // Try to store second – it should be caught and not overwrite.
        // We'll wrap in a try to confirm it doesn't corrupt.
        try
        {
            await Store.StoreResponseAsync(key, second, CancellationToken.None);
            await UnitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException) // unique constraint
        {
            // Expected, ignore
        }

        // Still returns first
        var stillFirst = await Store.GetResponseAsync(key, CancellationToken.None);
        stillFirst!.Result.Should().Be("first");
    }
}