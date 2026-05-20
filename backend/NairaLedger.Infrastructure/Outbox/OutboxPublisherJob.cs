using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NairaLedger.Application.EventHandlers;
using NairaLedger.Domain.DomianEvents;
using NairaLedger.Infrastructure.Persistence;
using System.Text.Json;

namespace NairaLedger.Infrastructure.Outbox;

/// <summary>
/// Hangfire job that publishes pending outbox messages.
/// </summary>
public class OutboxPublisherJob
{
    private readonly NairaLedgerDbContext _context;
    private readonly IMediator _mediator;

    public OutboxPublisherJob(NairaLedgerDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 5, 10, 30 })]
    public async Task ExecuteAsync()
    {
        var messages = await _context.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync();

        foreach (var msg in messages)
        {
            var domainEvent = DeserializeEvent(msg.EventType, msg.EventData);
            if (domainEvent is not null)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(notificationType, domainEvent);
                if (notification is not null)
                    await _mediator.Publish(notification);
            }

            msg.ProcessedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private static IDomainEvent? DeserializeEvent(string eventType, string json)
    {
        var type = Type.GetType(eventType);
        if (type is null) return null;
        return JsonSerializer.Deserialize(json, type) as IDomainEvent;
    }
}