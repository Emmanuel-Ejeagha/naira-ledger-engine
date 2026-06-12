using Microsoft.EntityFrameworkCore.Design;

namespace NairaLedger.Infrastructure.Persistence;

public class NairaLedgerDbContextFactory : IDesignTimeDbContextFactory<NairaLedgerDbContext>
{
    public NairaLedgerDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Database=NairaLedgerDev;Username=nairawallet;Password=nairawallet_dev";

        var optionsBuilder = new DbContextOptionsBuilder<NairaLedgerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new NairaLedgerDbContext(optionsBuilder.Options);
    }
}