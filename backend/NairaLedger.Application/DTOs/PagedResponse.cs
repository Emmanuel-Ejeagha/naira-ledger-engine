namespace NairaWallet.Application.DTOs;

/// <summary>
/// Standard response envelope for paginated lists.
/// </summary>
public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore);