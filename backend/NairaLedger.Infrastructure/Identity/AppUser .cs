using Microsoft.AspNetCore.Identity;

namespace NairaLedger.Infrastructure.Identity;

/// <summary>
/// Application user extending ASP.NET Identity. Additional profile fields can be added here.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>User's full name.</summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>When the user registered.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}