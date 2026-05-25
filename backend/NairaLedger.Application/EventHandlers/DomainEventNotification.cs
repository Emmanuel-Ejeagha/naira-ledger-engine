namespace NairaLedger.Application.EventHandlers;

public record DomainEventNotification<T>(T DomainEvent) : INotification
    where T : IDomainEvent;