using Microsoft.AspNetCore.Identity;

namespace NairaLedger.Infrastructure.Identity;

/// <summary>
/// Application role (User, Admin).
/// </summary>
public class AppRole : IdentityRole<Guid>
{
    public AppRole() { }

    public AppRole(string roleName) : base(roleName) { }
}