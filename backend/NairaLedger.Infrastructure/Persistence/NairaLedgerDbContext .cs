namespace NairaLedger.Infrastructure.Persistence;

/// <summary>
/// The main application DbContext combining domain aggregates, identity, and outbox.
/// </summary>
public class NairaLedgerDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public NairaLedgerDbContext(DbContextOptions<NairaLedgerDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.ApplyConfiguration(new WalletConfiguration());
        builder.ApplyConfiguration(new TransactionConfiguration());
        builder.ApplyConfiguration(new LedgerEntryConfiguration());
        builder.ApplyConfiguration(new IdempotencyRecordConfiguration());

        builder.Entity<OutboxMessage>(cfg =>
        {
            cfg.ToTable("OutboxMessages");
            cfg.HasKey(o => o.Id);
            cfg.Property(o => o.EventType).IsRequired().HasMaxLength(256);
            cfg.Property(o => o.EventData).IsRequired().HasColumnType("jsonb");
            cfg.Property(o => o.CreatedAt).IsRequired();
            cfg.HasIndex(o => o.ProcessedAt).HasFilter(null);
        });
    }
}