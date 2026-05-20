using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NairaLedger.Domain.Aggregates;
using NairaLedger.Domain.ValueObjects;
using NairaLedger.Infrastructure.Identity;
using NairaLedger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NairaLedger.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var context = scope.ServiceProvider.GetRequiredService<NairaLedgerDbContext>();

        // Ensure roles exist
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new AppRole("Admin"));
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new AppRole("User"));

        // Create admin user
        var adminEmail = "admin@nairawallet.ng";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "NairaWallet Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Create system bank user (for the bank float wallet)
        var systemEmail = "system@nairawallet.ng";
        AppUser? systemUser = await userManager.FindByEmailAsync(systemEmail);
        if (systemUser is null)
        {
            systemUser = new AppUser
            {
                UserName = systemEmail,
                Email = systemEmail,
                FullName = "NairaWallet System Bank",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(systemUser, "SystemPass!1");
        }

        // Create system bank wallet if not exists
        var bankWalletId = new Guid("00000000-0000-0000-0000-000000000001");
        if (!await context.Wallets.AnyAsync(w => w.Id == bankWalletId))
        {
            var bankWallet = new Wallet(new UserId(systemUser.Id), new WalletTag("Bank Float"));
            // Set the known ID for the bank wallet
            typeof(Wallet).GetProperty(nameof(Wallet.Id))!
                .SetValue(bankWallet, bankWalletId);

            context.Wallets.Add(bankWallet);
            await context.SaveChangesAsync();
        }
    }
}