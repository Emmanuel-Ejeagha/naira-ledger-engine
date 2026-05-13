using NairaLedger.Domain.ValueObjects;

namespace NairaLedger.Domain.DomianEvents;

/// <summary>
/// Raised when a user registers successfully.
/// </summary>
/// <param name="UserId">The unique identifier of the registered user.</param>
/// <param name="Email">The email address of the registered user.</param>
public record UserRegisteredEvent(UserId UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
