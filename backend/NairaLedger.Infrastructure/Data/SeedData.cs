namespace NairaLedger.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var context = scope.ServiceProvider.GetRequiredService<NairaLedgerDbContext>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new AppRole("Admin"));
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new AppRole("User"));

        var adminEmail = "admin@nairawallet.ng";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "NairaWallet Admin",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors)}");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException($"Failed to assign Admin role: {string.Join(", ", roleResult.Errors)}");
        }

        var systemEmail = "system@nairawallet.ng";
        var systemUser = await userManager.FindByEmailAsync(systemEmail);
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

        var bankWalletId = new Guid("00000000-0000-0000-0000-000000000001");
        if (!await context.Wallets.AnyAsync(w => w.Id == bankWalletId))
        {
            var bankWallet = new Wallet(new UserId(systemUser.Id), new WalletTag("Bank Float"));
            typeof(Wallet).GetProperty(nameof(Wallet.Id))!
                .SetValue(bankWallet, bankWalletId);

            context.Wallets.Add(bankWallet);
            await context.SaveChangesAsync();
        }
    }
}