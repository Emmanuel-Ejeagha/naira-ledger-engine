using MediatR;
using NairaLedger.Domain.DomianEvents;

namespace NairaLedger.Application.Commands.EventHandlers;

public record DomainEventNotification<T>(T DomainEvent) : INotification
    where T : IDomainEvent;