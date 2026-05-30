namespace NairaLedger.Tests.Infrastructure;

public class OutboxPublisherJobTests : IntegrationTestBase
{
    [Fact]
    public async Task Execute_ShouldProcessPendingMessages()
    {
        var db = ServiceProvider.GetRequiredService<NairaLedgerDbContext>();
        var job = ServiceProvider.GetRequiredService<OutboxPublisherJob>();

        var domainEvent = new WalletCreatedEvent(Guid.NewGuid(), new UserId(Guid.NewGuid()));
        var outboxMessage = new OutboxMessage
        {
            EventType = domainEvent.GetType().AssemblyQualifiedName!,
            EventData = JsonSerializer.Serialize(domainEvent)
        };
        db.OutboxMessages.Add(outboxMessage);
        await db.SaveChangesAsync();

        // Execute job – if handler registered, it will run; we only care that ProcessedAt is set.
        await job.ExecuteAsync();

        var processed = await db.OutboxMessages.FirstOrDefaultAsync(m => m.Id == outboxMessage.Id);
        processed.Should().NotBeNull();
        processed!.ProcessedAt.Should().NotBeNull();
    }
}