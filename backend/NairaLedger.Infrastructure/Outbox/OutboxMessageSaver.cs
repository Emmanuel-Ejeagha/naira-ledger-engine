using NairaLedger.Domain.BaseTypes;
using NairaLedger.Infrastructure.Persistence;
using System.Text.Json;

namespace NairaLedger.Infrastructure.Outbox;

/// <summary>
/// Intercepts domain events from aggregates during SaveChanges and persists them as outbox messages.
/// </summary>
public static class OutboxMessageSaver
{
    public static void SaveOutboxMessages(NairaLedgerDbContext context)
    {
        var aggregates = context.ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        foreach (var entry in aggregates)
        {
            foreach (var domainEvent in entry.Entity.DomainEvents)
            {
                var outboxMessage = new OutboxMessage
                {
                    EventType = domainEvent.GetType().AssemblyQualifiedName!,
                    EventData = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
                };
                context.OutboxMessages.Add(outboxMessage);
            }
            entry.Entity.ClearDomainEvents();
        }
    }
}