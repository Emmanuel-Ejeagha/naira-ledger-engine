namespace NairaLedger.Application.DTOs;

/// <summary>
/// Lightweight transaction representation for queries.
/// </summary>
public record TransactionDto(
    Guid TransactionId,
    string Reference,
    string Type,
    string Status,
    decimal Amount,
    DateTime CreatedAt);