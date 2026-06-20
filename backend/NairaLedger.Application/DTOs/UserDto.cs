namespace NairaLedger.Application.DTOs;

public record UserDto(Guid UserId, string Email, string FullName, bool EmailConfirmed);